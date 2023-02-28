#ifndef NATIVENES_CARTRIDGE_H
#define NATIVENES_CARTRIDGE_H

#include <memory>
#include "IMemory.h"

extern "C"
{
    struct __attribute__((__packed__)) program_ram_size_t
    {
        uint8_t multiply;

        [[nodiscard]] uint32_t bytes() const { return multiply * 8192; }
    };

    struct __attribute__((__packed__)) program_rom_size_t
    {
        uint8_t multiply;

        [[nodiscard]] uint32_t bytes() const { return multiply * 16384; }
    };

    struct __attribute__((__packed__)) character_rom_size_t
    {
        uint8_t multiply;

        [[nodiscard]] uint32_t bytes() const { return multiply * 8192; }
    };

    //https://www.nesdev.org/wiki/INES
    typedef struct __attribute__((__packed__)) ines_header_t
    {
        uint32_t magicNumber;
        struct program_ram_size_t programRomSize;
        struct character_rom_size_t characterRomSize;

        uint8_t verticalMirroring : 1;
        uint8_t hasPersistentProgramRam : 1;
        uint8_t hasTrainer : 1;
        uint8_t ignoreMirroring : 1;
        uint8_t lowerMapperNybble : 4;

        uint8_t unisystem : 1;
        uint8_t playChoice10 : 1;
        uint8_t nes20Format : 2;
        uint8_t upperMapperNybble : 4;

        struct program_ram_size_t programRamSize;

        uint8_t reserved;

        uint8_t tvSystem : 2;
        uint8_t unused : 2;
        uint8_t hasProgramRam : 1;
        uint8_t hasBusConflict : 1;
        uint8_t unused2 : 2;

        uint8_t mapper() const { return lowerMapperNybble | upperMapperNybble << 4; }
    } NesHeader;
#ifdef __linux__
    extern const NesHeader* const _getHeader();
    #define getHeader _getHeader
#else
    extern const NesHeader* const getHeader();
#endif
}

enum class NameTableMirroring
{
    Horizontal  = 0,
    Vertical    = 1,
    FourScreen  = 8,
    OneScreenLower,
    OneScreenHigher,
};

class CharacterRom : public IMemory
{
public:
    CharacterRom(uint8_t* data) : chrData(data) {}
    uint8_t Read(uint16_t address) override;
    void Write(uint16_t address, uint8_t data) override;
    
private:
    uint8_t* chrData;
};

class Cartridge : public IMemory
{
public:
    Cartridge(uint8_t* data, std::shared_ptr<CharacterRom>& characterData);
    ~Cartridge() override = default;
    uint8_t Read(uint16_t address) override;
    void Write(uint16_t address, uint8_t data) override;
    IMemory* Character() { return characterData.get(); }
    NameTableMirroring Mirroring() const { return nameTableMirroring; }

private:
    uint8_t* memData;
    std::shared_ptr<CharacterRom> characterData;
    NameTableMirroring nameTableMirroring;
    bool mirrorRom;
};

#endif //NATIVENES_CARTRIDGE_H
