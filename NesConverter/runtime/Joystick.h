#ifndef NATIVENES_JOYSTICK_H
#define NATIVENES_JOYSTICK_H

#include <cstdint>
#include <vector>

#define JOYSTICK_A 1
#define JOYSTICK_B 2
#define JOYSTICK_SELECT 4
#define JOYSTICK_START 8
#define JOYSTICK_UP 16
#define JOYSTICK_DOWN 32
#define JOYSTICK_LEFT 64
#define JOYSTICK_RIGHT 128

class Joystick
{
public:
    void Poll(uint8_t b);
    uint8_t Read();
    void KeyDown(uint8_t bit);
    void KeyUp(uint8_t bit);
private:
    uint8_t keyStates = 0;
    uint8_t joystick = 0;
    bool poll = false;
};

#endif //NATIVENES_JOYSTICK_H
