#include "CPU.h"

static IBus* memory;

#define NMIVector 0xfffa
#define ResetVector 0xfffc
#define IRQVector 0xfffe

extern "C"
{
#ifdef __linux__
    #define getValue _getValue
    #define getValue16 _getValue16
    #define setValue _setValue
    int _getValue(int address) { return memory->Read(address); }
    int _getValue16(int address) { return getValue(address) | (getValue(address + 1) << 8); }
    void _setValue(int address, uint8_t value) { memory->Write(address, value); }
#else
    int getValue(int address) { return memory->Read(address); }
    int getValue16(int address) { return getValue(address) | (getValue(address + 1) << 8); }
    void setValue(int address, uint8_t value) { memory->Write(address, value); }
#endif
}

namespace
{
    void pushStack(uint8_t& SP, uint8_t value)
    {
        memory->Write(0x100 | SP, value);
        --SP;
    }
}

uint8_t CPU::Step(IBus* mem)
{
    memory = mem;

    if (pendingNMI)
    {
        interruptSequence(NMI);
        pendingNMI = pendingIRQ = false;
        return 1;
    }
    else if (pendingIRQ)
    {
        interruptSequence(IRQ);
        pendingNMI = pendingIRQ = false;
        return 1;
    }

    state = mosCycle(state);
    memory = nullptr;
    return state.Cycles;
}

void CPU::Interrupt(InterruptType type)
{
    switch (type)
    {
        case InterruptType::NMI:
            pendingNMI = true;
            break;

        case InterruptType::IRQ:
            pendingIRQ = true;
            break;

        default:
            break;
    }
}


void CPU::interruptSequence(InterruptType type)
{
    if (state.Interrupt() && type != NMI && type != BRK_)
        return;

    if (type == BRK_)
        state.SetIP(state.GetIP() + 1);

    pushStack(state.SP, state.IP_high);
    pushStack(state.SP, state.IP_low);
    pushStack(state.SP, state.Flags &~0b11001111 | 0b10000 | (type == BRK_ ? 0b10000 : 0)) ;

    state.SetInterrupt();

    switch (type)
    {
        case IRQ:
        case BRK_:
            state.SetIP(memory->Read(IRQVector) | memory->Read(IRQVector+1) << 8);
            break;
        case NMI:
            state.SetIP(memory->Read(NMIVector) | memory->Read(NMIVector+1) << 8);
            break;
    }
}

void CPU::Reset(IBus* mem)
{
    uint16_t startAddr = mem->Read(ResetVector) | mem->Read(ResetVector+1) << 8;
    X() = 0;
    Y() = 0;
    A() = 0;
    state.Flags = 0;
    state.SetInterrupt();
    state.SetIP(startAddr);
    state.SP = 0xfd;
}
