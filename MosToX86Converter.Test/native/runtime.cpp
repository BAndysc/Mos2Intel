#include <iostream>
#include <fstream>
#include <cstdlib>
#include <functional>
#include "mos6502/mos6502.h"

uint8_t memory[0x10000];

uint8_t getValue(uint16_t addr)
{
    return memory[addr];
}

void setValue(uint16_t addr, uint8_t value)
{
    memory[addr] = value;
}

int main(int argc, char** argv)
{
    if (argc <= 1)
    {
        std::cout << "Usage: ./runtime <program> <start_address = 0>\n";
    }
    int starAddress = 0;
    if (argc >= 3)
    {
        starAddress = std::stoi(argv[2]);
    }
    // Loads the binary file stored in argv[1] into a unsigned char array
    std::ifstream file(argv[1], std::ios::binary | std::ios::ate);
    std::streamsize size = file.tellg();
    file.seekg(0, std::ios::beg);
    if (file.read((char*)memory, size))
    {
        // Creates a new mos6502 instance
        mos6502 cpu(getValue, setValue);
        // Runs the program
        uint64_t cycles = 0;
        memory[0xFFFC] = starAddress & 0xFF;
        memory[0xFFFD] = (starAddress >> 8) & 0xFF;
        cpu.Reset();
        do
        {
            cycles = 0;
            cpu.Run(0x7FFFFFFF, cycles, mos6502::INST_COUNT);
        } while (cycles > 0);
        std::cout << cycles << "\n";
    }
    else
    {
        std::cout << "Couldn't read file!";
    }
}