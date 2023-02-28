#ifndef NATIVENES_IMEMORY_H
#define NATIVENES_IMEMORY_H

#include <cstdint>

class IMemory
{
public:
    virtual ~IMemory() { };
    virtual uint8_t Read(uint16_t address) = 0;
    virtual void Write(uint16_t address, uint8_t data) = 0;
};

class IBus : public IMemory
{
};

#endif //NATIVENES_IMEMORY_H
