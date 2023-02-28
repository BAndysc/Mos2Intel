#ifndef NATIVENES_PICTUREBUS_H
#define NATIVENES_PICTUREBUS_H

#include <vector>
#include <stdexcept>
#include "Cartridge.h"
#include "PPU.h"

class PictureBus : public IBus
{
public:
    explicit PictureBus(std::shared_ptr<Cartridge>& cartridge);
    uint8_t Read(uint16_t addr) override;
    void Write(uint16_t addr, uint8_t value) override;
    ObjectAttribute& Sprite(uint8_t index) { return objectAttributes[index]; }

private:
    uint8_t palette[32];
    uint8_t ram[2048];
    uint8_t* nametables[4];
    ObjectAttribute objectAttributes[64];
    std::shared_ptr<Cartridge> cartridge;
};

#endif //NATIVENES_PICTUREBUS_H
