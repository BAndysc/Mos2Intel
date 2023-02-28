#include <stdexcept>
#include "Cartridge.h"

uint8_t Cartridge::Read(uint16_t address)
{
    if (address >= 0x6000 && address <= 0x7FFF)
    {
        return memData[address];
    }
    else if (address <= 0xBFFF)
    {
        return memData[address];
    }
    else if (address <= 0xFFFF)
    {
        if (mirrorRom)
            return memData[0x8000 + (address - 0xC000)];
        return memData[address];
    }
    throw std::runtime_error("Trying to Read unexpected address from cartridge");
}

void Cartridge::Write(uint16_t address, uint8_t data)
{
    if (address >= 0x6000 && address <= 0x7FFF)
    {
        memData[address] = data;
    }
    else if (address <= 0xBFFF)
    {
        memData[address] = data;
    }
    else if (address <= 0xFFFF)
    {
        if (mirrorRom)
            memData[0x8000 + (address - 0xC000)] = data;
        else
            memData[address] = data;
    }
    throw std::runtime_error("Trying to Read unexpected address from cartridge");
}

Cartridge::Cartridge(uint8_t* data, std::shared_ptr<CharacterRom>& characterData) : memData(data), characterData(characterData)
{
    auto header = getHeader();
    auto mapper = header->mapper();

    if (mapper != 0)
        throw std::runtime_error("Unsupported mapper");

    mirrorRom = header->programRomSize.multiply == 1;

    if (header->ignoreMirroring)
        nameTableMirroring = NameTableMirroring::FourScreen;
    else
        nameTableMirroring = header->verticalMirroring ? NameTableMirroring::Vertical : NameTableMirroring::Horizontal;
}

uint8_t CharacterRom::Read(uint16_t address)
{
    return chrData[address];
}

void CharacterRom::Write(uint16_t address, uint8_t data)
{
    chrData[address] = data;
}
