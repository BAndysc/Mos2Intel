#ifndef NATIVENES_PPU_H
#define NATIVENES_PPU_H

#include <cstdint>

enum VramAddressIncrement
{
    Add1Across = 0,
    Add32Down = 1,
};

enum PatternAddress
{
    Zero = 0,
    Ox1000 = 1
};

enum SpriteSize
{
    Regular = 0, // 8 x 8
    Double = 1 // 8 x 16
};

struct __attribute__((__packed__)) PPUControlView
{
    PPUControlView()
    {
        *(uint8_t*)this = 0;
    }

    PPUControlView(uint8_t bits)
    {
        *(uint8_t*)this = bits;
    }

    uint8_t nametable_x : 1;
    uint8_t nametable_y : 1;
    VramAddressIncrement vramIncrement : 1;
    PatternAddress spriteAddress : 1;
    PatternAddress backgroundAddress : 1;
    SpriteSize spriteSize : 1;
    bool masterSlave : 1;
    bool generateNMI : 1;

    uint16_t spriteHeight() const { return spriteSize == SpriteSize::Double ? 16 : 8; }
    uint16_t backgroundAddressOffset() const { return backgroundAddress == PatternAddress::Ox1000 ? 0x1000 : 0; }
    uint16_t spriteAddressOffset() const { return spriteAddress == PatternAddress::Ox1000 ? 0x1000 : 0; }
    uint8_t vramIncrementValue() const { return vramIncrement == VramAddressIncrement::Add32Down ? 32 : 1; }

    explicit operator uint8_t() const { return *(uint8_t*)this; }
};

struct __attribute__((__packed__)) PPUMaskView
{
    PPUMaskView()
    {
        *(uint8_t*)this = 0;
    }

    PPUMaskView(uint8_t bits)
    {
        *(uint8_t*)this = bits;
    }

    bool grayscale : 1;
    bool showBackgroundLeft : 1;
    bool showSpritesLeft : 1;
    bool showBackground : 1;
    bool showSprites : 1;
    bool emphasizeRed : 1;
    bool emphasizeGreen : 1;
    bool emphasizeBlue : 1;

    explicit operator uint8_t() const { return *(uint8_t*)this; }
};


struct __attribute__((__packed__)) PPUStatusView
{
    PPUStatusView()
    {
        *(uint8_t*)this = 0;
    }

    PPUStatusView(uint8_t bits)
    {
        *(uint8_t*)this = bits;
    }

    bool PPUOpenBus : 1;
    int unused : 4;
    bool spriteOverflow : 1;
    bool sprite0Hit : 1;
    bool vertBlankStarted : 1;

    explicit operator uint8_t() const { return *(uint8_t*)this; }
};

union __attribute__((__packed__)) TileNumber
{
    uint8_t Regular;
    struct __attribute__((__packed__))
    {
        PatternAddress bank : 1;
        uint8_t number : 7;
    } Double;
};

struct __attribute__((__packed__)) ObjectAttribute
{
    uint8_t y;
    TileNumber tileNumber;
    uint8_t palette : 2;
    uint8_t unused : 3;
    bool behindBackground : 1;
    bool flipHoriz : 1;
    bool flipVert : 1;
    uint8_t x;
    uint8_t paletteNumber() const { return palette << 2; }
};


union __attribute__((__packed__)) VRAMRegister
{
    struct __attribute__((__packed__))
    {
        uint8_t coarse_x : 5;
        uint8_t coarse_y : 5;
        uint8_t nametable_x : 1;
        uint8_t nametable_y : 1;
        uint8_t fine_y : 3;
        uint8_t unused : 1;
    };

    struct __attribute__((__packed__))
    {
        uint8_t low;
        uint8_t high;
    };

    uint16_t value = 0x0000;
};


#endif //NATIVENES_PPU_H
