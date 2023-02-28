#ifndef NATIVENES_SIMPLEPPU_H
#define NATIVENES_SIMPLEPPU_H

#include <functional>
#include <array>
#include "PictureBus.h"
#include "PPU.h"
#include "CPU.h"


struct RGB
{
    uint8_t R, G, B;

    unsigned int value() const
    {
        return ((uint32_t) R << 24) | ((uint32_t) G << 16) | (B << 8) | 0xFF;
    }
};

#define RES_Y  240
#define RES_X  256

class SimplePPU
{
public:
    SimplePPU(std::shared_ptr<PictureBus>& bus, std::function<void(RGB*, int, int, int, int)> &render, std::weak_ptr<CPU> cpu) :
            bus(bus), render(render), cpu(cpu), backbuffer(RES_X * RES_Y) { }

    void SetMask(PPUMaskView mask) { this->mask = mask; }
    uint8_t GetOAMData() const { return *(&bus->Sprite(0).y + oamAddress); }
    void SetOAMData(uint8_t value) { *(&bus->Sprite(0).y + oamAddress++) = value; }
    void SetOAMAddress(uint8_t addr) { oamAddress = addr; }

    void Step();
    void Reset();
    void DMA(const uint8_t* page_ptr);
    void SetControl(PPUControlView ctrl);
    void SetDataAddress(uint8_t addr);
    void SetScroll(uint8_t scroll);
    void SetData(uint8_t data);
    PPUStatusView GetStatus();
    uint8_t GetData();
private:
    std::weak_ptr<CPU> cpu;
    std::shared_ptr<PictureBus> bus;
    std::function<void(RGB*,  int, int, int, int)> render;
    std::vector<uint8_t> scanlineSprites;
    std::vector<RGB> backbuffer;

    PPUStatusView status;
    PPUMaskView mask;
    PPUControlView control;

    uint8_t fineXScroll;
    VRAMRegister tempAddrRegister;
    VRAMRegister dataAddress;
    uint8_t dataRegister;
    uint8_t oamAddress;
    bool writeHighByte;
    int cycle;
    int scanline;
    bool evenFrame;
};

#endif //NATIVENES_SIMPLEPPU_H
