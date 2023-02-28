#include <iostream>
#include <memory>
#include <chrono>
#include <SDL2/SDL.h>
#include <thread>
#include "Cartridge.h"
#include "NesBus.h"
#include "CPU.h"

#define WINDOW_WIDTH 256
#define WINDOW_HEIGHT 240
#define SCALE 3

using namespace std::literals;
using namespace std::chrono_literals;
using namespace std::literals::chrono_literals;

extern "C"
{
#ifdef __linux__
    extern uint8_t* _getMemory();
    extern uint8_t* _getCharacterRam();
    #define getMemory _getMemory
    #define getCharacterRam _getCharacterRam
#else
    extern uint8_t* getMemory();
    extern uint8_t* getCharacterRam();
#endif
};

#define CYCLES_PER_SEC 1789773



uint8_t KeySymToBit(SDL_Keycode sym)
{
    switch (sym)
    {
        case SDLK_w:
            return JOYSTICK_UP;
        case SDLK_s:
            return JOYSTICK_DOWN;
        case SDLK_a:
            return JOYSTICK_LEFT;
        case SDLK_d:
            return JOYSTICK_RIGHT;
        case SDLK_SPACE:
            return JOYSTICK_A;
        case SDLK_f:
            return JOYSTICK_B;
        case SDLK_RETURN:
            return JOYSTICK_START;
    }
    return 0;
}

#ifndef TITLE
#define TITLE "(run cmake with -DTITLE to set the game title ;))"
#endif

#define TICKS_FOR_NEXT_FRAME (1000 / 60)
int lastTime = 0;

int main()
{
    SDL_Event event;
    SDL_Renderer *renderer;
    SDL_Window *window;
    int i;

    SDL_Init(SDL_INIT_VIDEO);
    //SDL_RENDERER_PRESENTVSYNC
    SDL_CreateWindowAndRenderer(WINDOW_WIDTH * SCALE, WINDOW_HEIGHT * SCALE, 0, &window, &renderer);
    SDL_SetWindowTitle(window, TITLE " :: Native x86-64");

    SDL_Texture* backbuffer = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_RGBA8888, SDL_TEXTUREACCESS_STREAMING, WINDOW_WIDTH * SCALE, WINDOW_HEIGHT * SCALE);
    if (!backbuffer) {
        SDL_LogError(SDL_LOG_CATEGORY_APPLICATION, "Couldn't set create texture: %s\n", SDL_GetError());
        return -1;
    }

    SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
    SDL_RenderClear(renderer);

    std::function<void(RGB*, int, int, int, int)> render = [&](RGB* screen, int startX, int startY, int width, int height) {
        while (SDL_GetTicks()-lastTime < TICKS_FOR_NEXT_FRAME)
        {
            SDL_Delay(1);
        }
        lastTime = SDL_GetTicks();
        void *pixels;
        int pitch;
        SDL_LockTexture(backbuffer, NULL, &pixels, &pitch);
        for (int j = startY; j < startY + height; ++j)
        {
            uint32_t* ptr = (uint32_t*)((uint8_t*)pixels + 3 * j * pitch);
            uint32_t* ptr2 = (uint32_t*)((uint8_t*)pixels + (3 * j+1) * pitch);
            uint32_t* ptr3 = (uint32_t*)((uint8_t*)pixels + (3 * j + 2) * pitch);
            for (int i = 0; i < 256; ++i)
            {
                for (int k = 0; k < 3; ++k)
                {
                    *ptr++ = screen[i + j * WINDOW_WIDTH].value();
                    *ptr2++ = screen[i + j * WINDOW_WIDTH].value();
                    *ptr3++ = screen[i + j * WINDOW_WIDTH].value();
                }
            }
        }
        SDL_UnlockTexture(backbuffer);
        SDL_RenderClear(renderer);
        SDL_RenderCopy(renderer,  backbuffer, NULL, NULL);
        SDL_RenderPresent(renderer);
    };

    std::shared_ptr<Joystick> controller = std::make_shared<Joystick>();
    std::shared_ptr<CharacterRom> characterRom = std::make_shared<CharacterRom>(getCharacterRam());
    std::shared_ptr<Cartridge> cardrige = std::make_shared<Cartridge>(getMemory(), characterRom);
    std::shared_ptr<PictureBus> pictureBus = std::make_shared<PictureBus>(cardrige);
    std::shared_ptr<CPU> cpu = std::make_shared<CPU>();
    std::shared_ptr<SimplePPU> ppu = std::make_shared<SimplePPU>(pictureBus, render, (std::weak_ptr<CPU>)cpu);
    std::shared_ptr<NesBus> nesBus = std::make_shared<NesBus>(cardrige, ppu, controller);

    cpu->Reset(nesBus.get());
    ppu->Reset();

    std::chrono::time_point<std::chrono::system_clock> prev = std::chrono::system_clock::now();
    while (true) {
        if (SDL_PollEvent(&event))
        {
            if (event.type == SDL_QUIT)
                break;
            if (event.type == SDL_KEYDOWN)
            {
                auto bit = KeySymToBit(event.key.keysym.sym);
                controller->KeyDown(bit);
            }
            else if (event.type == SDL_KEYUP)
            {
                auto bit = KeySymToBit(event.key.keysym.sym);
                controller->KeyUp(bit);
            }
        }
        std::chrono::time_point<std::chrono::system_clock> now = std::chrono::system_clock::now();
        auto diff = now - prev;
        prev = now;

        std::chrono::high_resolution_clock::duration time{};

        while (time < diff)
        {
            int cycles = cpu->Step(nesBus.get());
            for (int i = 0; i < cycles * 3; ++i)
                ppu->Step();

            time += cycles * std::chrono::nanoseconds(559);
        }
    }
    SDL_DestroyRenderer(renderer);
    SDL_DestroyWindow(window);
    SDL_Quit();
    return EXIT_SUCCESS;
}
