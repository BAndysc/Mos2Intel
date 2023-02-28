#ifndef NATIVENES_NESBUS_H
#define NATIVENES_NESBUS_H

#include <cstdint>
#include <vector>
#include <stdexcept>
#include <memory>
#include "IMemory.h"
#include "SimplePPU.h"
#include "Joystick.h"

#define RAM_SIZE 0x800

class NesBus : public IBus
{
public:
    NesBus(std::shared_ptr<Cartridge>& cardrige,
           std::shared_ptr<SimplePPU>& ppu,
           std::shared_ptr<Joystick>& controller)
        : cardrige(cardrige),
          ppu(ppu),
          controller(controller) {
        ram.resize(RAM_SIZE);
    }

    uint8_t Read(uint16_t address) override;
    void Write(uint16_t address, uint8_t data) override;


private:
    std::vector<uint8_t> ram;
    std::shared_ptr<Cartridge> cardrige;
    std::shared_ptr<SimplePPU> ppu;
    std::shared_ptr<Joystick> controller;
};


#endif //NATIVENES_NESBUS_H
