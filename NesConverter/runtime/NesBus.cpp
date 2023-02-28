#include "NesBus.h"
#include <cstdlib>

// https://www.nesdev.org/wiki/CPU_memory_map

enum MemoryMappedRegisters
{
    PPUCTRL = 0x2000,
    PPUMASK,
    PPUSTATUS,
    OAMADDR,
    OAMDATA,
    PPUSCROL,
    PPUADDR,
    PPUDATA,
    OAMDMA = 0x4014,
    JOY1 = 0x4016,
    JOY2 = 0x4017,
};
uint8_t NesBus::Read(uint16_t address)
{
    if (address <= 0x1FFF)
        return ram[address & 0x7FF];
    else if (address <= 0x3FFF)
    {
        address = 0x2000 + (address & 0x7);
        if (address == PPUSTATUS)
            return (uint8_t) ppu->GetStatus();
        if (address == PPUDATA)
            return ppu->GetData();
        if (address == OAMADDR)
            return ppu->GetOAMData();
        return 0;
        //throw std::runtime_error("Unexpected PPU register Read!");
    }
    else if (address <= 0x4017)
    {
        // I/O
        if (address == JOY1)
            return controller->Read();

        return 0;
        //throw std::runtime_error("trying to Read io address");
    }
    else if (address <= 0x401F)
    {
        throw std::runtime_error("trying to Read test address");
    }
    else
    {
        return cardrige->Read(address);
    }
}

void NesBus::Write(uint16_t address, uint8_t data)
{
    if (address <= 0x1FFF)
        ram[address & 0x7FF] = data;
    else if (address <= 0x3FFF)
    {
        address = 0x2000 + (address & 0x7);
        if (address == PPUCTRL)
            ppu->SetControl(data);
        else if (address == PPUMASK)
            ppu->SetMask(data);
        else if (address == PPUADDR)
            ppu->SetDataAddress(data);
        else if (address == PPUSCROL)
            ppu->SetScroll(data);
        else if (address == PPUDATA)
            ppu->SetData(data);
        else if (address == OAMADDR)
            ppu->SetOAMAddress(data);
        else if (address == OAMDATA)
            ppu->SetOAMData(data);
        //else
            //throw std::runtime_error("Unexpected PPU register write!");
    }
    else if (address <= 0x4017)
    {
        if (address == OAMDMA)
        {
            uint16_t addr = data << 8;
            auto page_ptr = &ram[addr & 0x7ff];
            if (page_ptr != nullptr)
            {
                ppu->DMA(page_ptr);
            }
            else
            {
            }
        }
        else if (address == JOY1)
            controller->Poll(data);
        // I/O
    }
    else if (address <= 0x401F)
    {
        throw std::runtime_error("trying to write test address");
    }
    else
    {
        cardrige->Write(address, data);
    }
}
