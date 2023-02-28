#include "PictureBus.h"

PictureBus::PictureBus(std::shared_ptr<Cartridge>& cartridge) : cartridge(cartridge)
{
    switch (cartridge->Mirroring())
    {
        case NameTableMirroring::Horizontal:
            nametables[0] = nametables[1] = ram;
            nametables[2] = nametables[3] = ram + 0x400;
            break;
        case NameTableMirroring::Vertical:
            nametables[0] = nametables[2] = ram;
            nametables[1] = nametables[3] = ram + 0x400;
            break;
        case NameTableMirroring::OneScreenLower:
            nametables[0] = nametables[1] = nametables[2] = nametables[3] = ram;
            break;
        case NameTableMirroring::OneScreenHigher:
            nametables[0] = nametables[1] = nametables[2] = nametables[3] = ram + 0x400;
            break;
        case NameTableMirroring::FourScreen:
            throw std::runtime_error("not supported");
    }
}

uint8_t PictureBus::Read(uint16_t addr)
{
    if (addr <= 0x1FFF)
        return cartridge->Character()->Read(addr);
    else if (addr <= 0x23FF)
        return nametables[0][addr - 0x2000];
    else if (addr <= 0x27FF)
        return nametables[1][addr - 0x2400];
    else if (addr <= 0x2BFF)
        return nametables[2][addr - 0x2800];
    else if (addr <= 0x2FFF)
        return nametables[3][addr - 0x2C00];
    else if (addr <= 0x33FF)
        return nametables[0][addr - 0x3000];
    else if (addr <= 0x37FF)
        return nametables[1][addr - 0x3400];
    else if (addr <= 0x3BFF)
        return nametables[2][addr - 0x3800];
    else if (addr <= 0x3EFF)
        return nametables[3][addr - 0x3C00];
    else if (addr < 0x3fff)
    {
        auto index = addr & 0x1f;
        // Addresses $3F10/$3F14/$3F18/$3F1C are mirrors of $3F00/$3F04/$3F08/$3F0C
        if (index >= 0x10 && addr % 4 == 0)
            index = index & 0xf;
        return palette[index];
    }
    return 0;
}

void PictureBus::Write(uint16_t addr, uint8_t value)
{
    if (addr < 0x2000)
        cartridge->Character()->Write(addr, value);
    else if (addr <= 0x23FF)
        nametables[0][addr - 0x2000] = value;
    else if (addr <= 0x27FF)
        nametables[1][addr - 0x2400] = value;
    else if (addr <= 0x2BFF)
        nametables[2][addr - 0x2800] = value;
    else if (addr <= 0x2FFF)
        nametables[3][addr - 0x2C00] = value;
    else if (addr <= 0x33FF)
        nametables[0][addr - 0x3000] = value;
    else if (addr <= 0x37FF)
        nametables[1][addr - 0x3400] = value;
    else if (addr <= 0x3BFF)
        nametables[2][addr - 0x3800] = value;
    else if (addr <= 0x3EFF)
        nametables[3][addr - 0x3C00] = value;
    else if (addr < 0x3fff)
    {
        auto index = addr & 0x1f;
        // Addresses $3F10/$3F14/$3F18/$3F1C are mirrors of $3F00/$3F04/$3F08/$3F0C
        if (index >= 0x10 && addr % 4 == 0)
            index = index & 0xf;
        palette[index] = value;
    }
}
