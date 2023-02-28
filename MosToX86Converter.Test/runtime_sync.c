#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>
#include <time.h>
#include <unistd.h>
#include <stdint.h>
#include <stdbool.h>

typedef struct __attribute__((__packed__)) mos_state_t
{
    unsigned char Cycles; // liczba cykli ostatniej instrukcji
    unsigned char IP_low; // dolny bajt adresu instrukcji
    unsigned char IP_high;// górny bajt wskaźnika instrukcji
    unsigned char SP;     // dolne osiem bitów rejestru stosu
    unsigned char Flags;  // flagi procesora MOS
    unsigned char Y;      // wartość rejestru Y
    unsigned char X;      // wartość rejestru X
    unsigned char A;      // wartość rejestru A
} MosState;

bool IsHalt(MosState state)
{
    return state.Cycles & 0x80;
}

unsigned char* memory;


#ifdef __APPLE__
extern void* getMemory();
extern MosState mosCycle(MosState state);
extern MosState mos6502();
extern void mosInit();
#else
extern void* _getMemory();
extern MosState _mosCycle(MosState state);
extern MosState _mos6502();
extern void _mosInit();

void* getMemory()
{
    return _getMemory();
}

#ifdef CYCLES
MosState mosCycle(MosState state)
{
    return _mosCycle(state);
}
#endif

MosState mos6502()
{
    return _mos6502();
}

void mosInit()
{
    _mosInit();
}

#endif

unsigned char getValue(int address)
{
    return *(memory + (address & 0xFFFF));
}

unsigned short getValue16(int address)
{
    return (unsigned short)getValue(address) | (unsigned short)getValue(address + 1) << 8;
}

void setValue(int address, unsigned char value)
{
    *(memory + (address & 0xFFFF)) = value;
}

void debugMos(MosState cpu)
{
    unsigned char A = cpu.A;
    unsigned char X = cpu.X;
    unsigned char Y = cpu.Y;
    unsigned char Flags = cpu.Flags;
    printf("%04X A:%02X X:%02X Y:%02X SP:%02X\n", cpu.IP_low | cpu.IP_high << 8, A, X, Y, cpu.SP);
}

#ifndef __APPLE__

unsigned char _getValue(int address)
{
    return getValue(address);
}

unsigned short _getValue16(int address)
{
    return getValue16(address);
}

void _setValue(int address, unsigned char value)
{
    return setValue(address, value);
}

void _debugMos(MosState cpu)
{
    debugMos(cpu);
}

#endif

int main(void)
{
    mosInit();
    memory = getMemory();
#ifdef CYCLES
    MosState cpu = {0};
    cpu.IP_high = START_INSTR >> 8;
    cpu.IP_low = START_INSTR & 0xFF;
    cpu.Flags = 16 | 32; // Unused | B
    #if CYCLES == 0
        while (!IsHalt(cpu))
            cpu = mosCycle(cpu);
    #else
        for (int i = 0; i < CYCLES && !IsHalt(cpu); ++i)
            cpu = mosCycle(cpu);
    #endif
#else
    MosState cpu = mos6502();
#endif

    unsigned char A = cpu.A;
    unsigned char X = cpu.X;
    unsigned char Y = cpu.Y;
    unsigned char Flags = cpu.Flags;
    printf("%02X %02X %02X %02X\n", A, X, Y, Flags);
    for (int i = 0; i < 0xFFFF; ++i)
    {
        printf("%02X ", memory[i]);
        if ((i + 1) % 16 == 0)
            printf("\n");
    }
}