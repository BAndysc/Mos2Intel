#include "Joystick.h"

void Joystick::KeyDown(uint8_t bit)
{
    joystick |= bit;
}

void Joystick::KeyUp(uint8_t bit)
{
    joystick &= ~bit;
}

void Joystick::Poll(uint8_t b)
{
    poll = !!b;
    if (poll)
        keyStates = joystick;
}

uint8_t Joystick::Read()
{
    uint8_t value;
    if (!poll)
    {
        value = (keyStates & 1);
        keyStates = keyStates >> 1;
    }
    else
        value = (joystick & JOYSTICK_A) != 0;
    return value | 0x40;
}
