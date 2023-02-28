#ifndef NATIVENES_CPU_H
#define NATIVENES_CPU_H

#include <cstdint>
#include "IMemory.h"

enum InterruptType
{
    IRQ,
    NMI,
    BRK_
};

extern "C"
{
    typedef struct __attribute__((__packed__)) mos_state_t
    {
        uint8_t Cycles;
        uint8_t IP_low;
        uint8_t IP_high;
        uint8_t SP;
        uint8_t Flags;
        uint8_t Y;
        uint8_t X;
        uint8_t A;

        [[nodiscard]] uint16_t GetIP() const
        {
            return (uint16_t)IP_low | (uint16_t)IP_high << 8;
        }

        void SetIP(uint16_t ip)
        {
            IP_low = ip & 0xFF;
            IP_high = ip >> 8;
        }

        void SetInterrupt()
        {
            Flags |= 4;
        }

        bool Interrupt() const { return Flags & 4; }
    } MosState;

    
#ifdef __linux__
    extern MosState _mosCycle(MosState state);
    extern void _mosInit();
    #define mosCycle _mosCycle
    #define mosInit _mosInit
#else
    extern MosState mosCycle(MosState state);
    extern void mosInit();
#endif
}

class CPU {
public:
    CPU() { mosInit(); }
    uint8_t& X() { return state.X; }
    uint8_t& A() { return state.A; }
    uint8_t& Y() { return state.Y; }

    uint8_t Step(IBus* memory);
    void Reset(IBus* bus);
    void Interrupt(InterruptType type);
    void interruptSequence(InterruptType type);

private:
    MosState state = {0};
    bool pendingNMI = false;
    bool pendingIRQ = false;
};


#endif //NATIVENES_CPU_H
