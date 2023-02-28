#include <cstring>
#include "SimplePPU.h"

//https://www.nesdev.org/wiki/PPU_palettes
const RGB colors[] = {
        {0x66, 0x66, 0x66},
        {0x00, 0x2a, 0x88},
        {0x14, 0x12, 0xa7},
        {0x3b, 0x00, 0xa4},
        {0x5c, 0x00, 0x7e},
        {0x6e, 0x00, 0x40},
        {0x6c, 0x06, 0x00},
        {0x56, 0x1d, 0x00},

        {0x33, 0x35, 0x00},
        {0x0b, 0x48, 0x00},
        {0x00, 0x52, 0x00},
        {0x00, 0x4f, 0x08},
        {0x00, 0x40, 0x4d},
        {0x00, 0x00, 0x00},
        {0x00, 0x00, 0x00},
        {0x00, 0x00, 0x00},

        {0xad, 0xad, 0xad},
        {0x15, 0x5f, 0xd9},
        {0x42, 0x40, 0xff},
        {0x75, 0x27, 0xfe},
        {0xa0, 0x1a, 0xcc},
        {0xb7, 0x1e, 0x7b},
        {0xb5, 0x31, 0x20},
        {0x99, 0x4e, 0x00},

        {0x6b, 0x6d, 0x00},
        {0x38, 0x87, 0x00},
        {0x0c, 0x93, 0x00},
        {0x00, 0x8f, 0x32},
        {0x00, 0x7c, 0x8d},
        {0x00, 0x00, 0x00},
        {0x00, 0x00, 0x00},
        {0x00, 0x00, 0x00},

        {0xff, 0xfe, 0xff},
        {0x64, 0xb0, 0xff},
        {0x92, 0x90, 0xff},
        {0xc6, 0x76, 0xff},
        {0xf3, 0x6a, 0xff},
        {0xfe, 0x6e, 0xcc},
        {0xfe, 0x81, 0x70},
        {0xea, 0x9e, 0x22},

        {0xbc, 0xbe, 0x00},
        {0x88, 0xd8, 0x00},
        {0x5c, 0xe4, 0x30},
        {0x45, 0xe0, 0x82},
        {0x48, 0xcd, 0xde},
        {0x4f, 0x4f, 0x4f},
        {0x00, 0x00, 0x00},
        {0x00, 0x00, 0x00},

        {0xff, 0xfe, 0xff},
        {0xc0, 0xdf, 0xff},
        {0xd3, 0xd2, 0xff},
        {0xe8, 0xc8, 0xff},
        {0xfb, 0xc2, 0xff},
        {0xfe, 0xc4, 0xea},
        {0xfe, 0xcc, 0xc5},
        {0xf7, 0xd8, 0xa5},

        {0xe4, 0xe5, 0x94},
        {0xcf, 0xef, 0x96},
        {0xbd, 0xf4, 0xab},
        {0xb3, 0xf3, 0xcc},
        {0xb5, 0xeb, 0xf2},
        {0xb8, 0xb8, 0xb8},
        {0x00, 0x00, 0x00},
        {0x00, 0x00, 0x00}
};

void SimplePPU::Reset()
{
    mask.showBackground = mask.showSprites = true;
    dataAddress.value = cycle = scanline = oamAddress = fineXScroll = tempAddrRegister.value = 0;
    control.vramIncrement = VramAddressIncrement::Add32Down;
    scanline = -1;
    scanlineSprites.reserve(8);
    scanlineSprites.resize(0);
}

void SimplePPU::DMA(const uint8_t *page_ptr)
{
    std::memcpy(&bus->Sprite(0).y, page_ptr + 256 - oamAddress, oamAddress);
    std::memcpy(&bus->Sprite(0).y + oamAddress, page_ptr, 256 - oamAddress);
}

void SimplePPU::SetControl(PPUControlView ctrl)
{
    control = ctrl;
    tempAddrRegister.nametable_x = ctrl.nametable_x;
    tempAddrRegister.nametable_y = ctrl.nametable_y;
}

PPUStatusView SimplePPU::GetStatus()
{
    PPUStatusView value = status;
    status.vertBlankStarted = false;
    writeHighByte = true; // reading status register, resets write byte stage
    return value;
}

void SimplePPU::SetScroll(uint8_t scroll)
{
    if (writeHighByte)
    {
        fineXScroll = scroll & 0x7;
        tempAddrRegister.coarse_x = scroll >> 3;
        writeHighByte = false;
    }
    else
    {
        tempAddrRegister.fine_y = scroll & 0x7;
        tempAddrRegister.coarse_y = scroll >> 3;
        writeHighByte = true;
    }
}

void SimplePPU::SetDataAddress(uint8_t addr)
{
    if (writeHighByte)
    {
        tempAddrRegister.high = addr & 0x3f;
        writeHighByte = false;
    }
    else
    {
        dataAddress.high = tempAddrRegister.high;
        dataAddress.low = tempAddrRegister.low = addr;
        writeHighByte = true;
    }
}

uint8_t SimplePPU::GetData()
{
    auto data = bus->Read(dataAddress.value);
    dataAddress.value += control.vramIncrementValue();

    //Reads are delayed by one byte/Read when address is in this range
    if (dataAddress.value < 0x3f00)
        std::swap(data, dataRegister);

    return data;
}

void SimplePPU::SetData(uint8_t data)
{
    bus->Write(dataAddress.value, data);
    dataAddress.value += control.vramIncrementValue();
}

// https://www.nesdev.org/wiki/PPU_rendering
void SimplePPU::Step()
{
    if (scanline == -1) // prerender
    {
        if (cycle == 1)
            status.vertBlankStarted = status.sprite0Hit = false;
        else if (cycle == 258 && mask.showBackground && mask.showSprites)
        {
            dataAddress.coarse_x = tempAddrRegister.coarse_x;
            dataAddress.nametable_x = tempAddrRegister.nametable_x;
        }
        else if (cycle > 280 && cycle <= 304 && mask.showBackground && mask.showSprites)
        {
            dataAddress.coarse_y = tempAddrRegister.coarse_y;
            dataAddress.nametable_y = tempAddrRegister.nametable_y;
            dataAddress.fine_y = tempAddrRegister.nametable_y;
        }
    }
    else if (scanline <= 239) // visible frames
    {
        if (cycle == 0)
        {
            //idle
        }
        else if (cycle <= 256)
        {
            bool backgroundOpaque = false;
            bool spriteOpaque = true;
            bool spriteForeground = false;
            uint8_t backgroundColor = 0;
            uint8_t spriteColor = 0;

            int x = cycle - 1;
            int y = scanline;

            if (mask.showBackground)
            {
                auto xFine = (fineXScroll + x) % 8;
                if (mask.showBackgroundLeft || x >= 8)
                {
                    uint16_t addr = 0x2000 | (dataAddress.value & 0x0FFF);
                    uint8_t tile = bus->Read(addr);

                    addr = (tile * 16) + dataAddress.fine_y;
                    addr += control.backgroundAddressOffset();
                    backgroundColor = (bus->Read(addr) >> (7 ^ xFine)) & 1;
                    backgroundColor |= ((bus->Read(addr + 8) >> (7 ^ xFine)) & 1) << 1;

                    backgroundOpaque = backgroundColor;

                    addr = 0x23C0 | (dataAddress.nametable_x << 10) | (dataAddress.nametable_y << 11) | ((dataAddress.value >> 4) & 0x38) | ((dataAddress.value >> 2) & 0x07);
                    auto attribute = bus->Read(addr);
                    int shift = ((dataAddress.value >> 4) & 4) | (dataAddress.value & 2);
                    backgroundColor |= ((attribute >> shift) & 0x3) << 2;
                }

                if (xFine == 7)
                {
                    if (dataAddress.coarse_x < 31)
                        dataAddress.coarse_x += 1;
                    else
                    {
                        dataAddress.coarse_x = 0;
                        dataAddress.nametable_x = !dataAddress.nametable_x;
                    }
                }
            }

            if (mask.showSprites && (mask.showSpritesLeft || x >= 8))
            {
                for (auto i : scanlineSprites)
                {
                    ObjectAttribute& sprite = bus->Sprite(i);
                    uint8_t spriteX = sprite.x;
                    if (0 > x - spriteX || x - spriteX >= 8)
                        continue;

                    uint8_t spriteY = sprite.y + 1;
                    TileNumber tile = sprite.tileNumber;

                    int xShift = (x - spriteX) % 8, y_offset = (y - spriteY) % control.spriteHeight();

                    if (!sprite.flipHoriz)
                        xShift ^= 7;
                    if (sprite.flipVert)
                        y_offset ^= (control.spriteHeight() - 1);

                    uint16_t addr;
                    if (control.spriteSize == SpriteSize::Regular)
                        addr = tile.Regular * 16 + y_offset + control.spriteAddressOffset();
                    else
                    {
                        y_offset = (y_offset & 7) | ((y_offset & 8) << 1);
                        addr = tile.Double.number * 32 + y_offset + tile.Double.bank == PatternAddress::Ox1000 ? 0x1000 : 0;
                    }

                    spriteColor |= (bus->Read(addr) >> xShift) & 1;
                    spriteColor |= ((bus->Read(addr + 8) >> xShift) & 1) << 1;

                    spriteOpaque = spriteColor;
                    if (!spriteOpaque)
                    {
                        spriteColor = 0;
                        continue;
                    }

                    spriteColor = spriteColor | 0x10 | sprite.paletteNumber();
                    spriteForeground = !sprite.behindBackground;

                    if (!status.sprite0Hit && mask.showBackground && i == 0 && backgroundOpaque)
                        status.sprite0Hit = true;
                    break;
                }
            }

            uint8_t paletteAddr = backgroundColor;

            if ((!backgroundOpaque && spriteOpaque) || (backgroundOpaque && spriteOpaque && spriteForeground))
                paletteAddr = spriteColor;
            else if (!backgroundOpaque)
                paletteAddr = 0;

            backbuffer[x + y * RES_X] = colors[bus->Read(paletteAddr + 0x3F00)];
        }
        else if (cycle == 257)
        {
            if (mask.showBackground || mask.showSprites)
            {
                if (dataAddress.fine_y < 7)
                    dataAddress.fine_y++;
                else
                {
                    dataAddress.fine_y = 0;
                    if (dataAddress.coarse_y == 29)
                    {
                        dataAddress.coarse_y = 0;
                        dataAddress.nametable_y = !dataAddress.nametable_y;
                    }
                    else if (dataAddress.coarse_y == 31)
                        dataAddress.coarse_y = 0;
                    else
                        dataAddress.coarse_y += 1;
                }
            }
        }
        else if (cycle == 258)
        {
            if (mask.showBackground || mask.showSprites)
            {
                dataAddress.coarse_x = tempAddrRegister.coarse_x;
                dataAddress.nametable_x = tempAddrRegister.nametable_x;
            }
        }
        else if (cycle <= 320)
        {
        }
        else if (cycle <= 336)
        {
        }
        else if (cycle == 340)
        {
            scanlineSprites.clear();
            for (int spriteIndex = 0; spriteIndex < 64; ++spriteIndex)
            {
                uint8_t spriteY = bus->Sprite(spriteIndex).y;
                if (scanline >= spriteY && scanline < control.spriteHeight() + spriteY)
                {
                    if (scanlineSprites.size() >= 8)
                    {
                        status.spriteOverflow = true;
                        break;
                    }
                    scanlineSprites.push_back(spriteIndex);
                }
            }
        }
    }
    else if (scanline == 240) // post render
    {
        if (cycle == 0)
            render(backbuffer.data(), 0, 0, 256, 240);
    }
    else if (scanline <= 260) // v blank
    {
        if (cycle == 1 && scanline == 241)
        {
            status.vertBlankStarted = true;
            if (control.generateNMI)
                cpu.lock()->Interrupt(InterruptType::NMI);
        }
    }
    cycle++;
    if (cycle == 341 || (scanline == -1 && evenFrame && cycle == 340))
    {
        cycle = 0;
        scanline++;
        if (scanline == 261)
            scanline = -1;
        evenFrame = !evenFrame;
    }
}
