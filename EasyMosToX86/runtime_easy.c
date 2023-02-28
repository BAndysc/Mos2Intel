#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>
#include <time.h>
#include <unistd.h>
#include <stdint.h>
#include <stdbool.h>

// SDL
#include <SDL2/SDL.h>
#include <SDL2/SDL_timer.h>

typedef struct __attribute__((__packed__)) mos_state_t
{
    unsigned char Cycles;
    unsigned char IP_low;
    unsigned char IP_high;
    unsigned char SP;
    unsigned char Flags;
    unsigned char Y;
    unsigned char X;
    unsigned char A;
} MosState;

bool IsHalt(MosState state)
{
    return state.Cycles & 0x80;
}

extern void* getMemory();
extern MosState mosCycle(MosState state);
extern void mosInit();

#define START_INSTR 0x600

unsigned char PALETTE[][4] = {
{0x00, 0x00, 0x00, 0xFF},
{0xff, 0xff, 0xff, 0xFF},
{0x88, 0x00, 0x00, 0xFF},
{0xaa, 0xff, 0xee, 0xFF},
{0xcc, 0x44, 0xcc, 0xFF},
{0x00, 0xcc, 0x55, 0xFF},
{0x00, 0x00, 0xaa, 0xFF},
{0xee, 0xee, 0x77, 0xFF},
{0xdd, 0x88, 0x55, 0xFF},
{0x66, 0x44, 0x00, 0xFF},
{0xff, 0x77, 0x77, 0xFF},
{0x33, 0x33, 0x33, 0xFF},
{0x77, 0x77, 0x77, 0xFF},
{0xaa, 0xff, 0x66, 0xFF},
{0x00, 0x88, 0xff, 0xFF},
{0xbb, 0xbb, 0xbb, 0xFF}
};

int main(void)
{
    srand(time(0));
    mosInit();
    unsigned char* memory = getMemory();

    MosState cpu = {0};
    cpu.IP_high = START_INSTR >> 8;
    cpu.IP_low = START_INSTR & 0xFF;
    cpu.Flags = 0x16 | 0x32; // Unused | B
    
// SDL

   // returns zero on success else non-zero
    if (SDL_Init(SDL_INIT_EVERYTHING) != 0) {
        printf("error initializing SDL: %s\n", SDL_GetError());
    }
    SDL_Window* win = SDL_CreateWindow("GAME", // creates a window
                                       SDL_WINDOWPOS_CENTERED,
                                       SDL_WINDOWPOS_CENTERED,
                                       32 * 24, 32 * 24, 0);
 
    Uint32 render_flags = SDL_RENDERER_ACCELERATED;
 
    SDL_Renderer* rend = SDL_CreateRenderer(win, -1, render_flags);
    
    // controls animation loop
    int close = 0;
 
    // speed of box
    int speed = 300;
 
    // animation loop
    while (!close) {
        SDL_Event event;
 
        // Events management
        while (SDL_PollEvent(&event)) {
            switch (event.type) {
 
            case SDL_QUIT:
                // handling of close button
                close = 1;
                break;
 
            case SDL_KEYDOWN:
                // keyboard API for key pressed
                switch (event.key.keysym.scancode) {
                case SDL_SCANCODE_W:
                case SDL_SCANCODE_UP:
                    memory[0xFF] = 'w';
                    break;
                case SDL_SCANCODE_A:
                case SDL_SCANCODE_LEFT:
                    memory[0xFF] = 'a';
                    break;
                case SDL_SCANCODE_S:
                case SDL_SCANCODE_DOWN:
                    memory[0xFF] = 's';
                    break;
                case SDL_SCANCODE_D:
                case SDL_SCANCODE_RIGHT:
                    memory[0xFF] = 'd';
                    break;
                default:
                    break;
                }
            }
        }
        
        memory[0xFE] = rand();
        SDL_Rect dest; 
        dest.w = 24;
        dest.h = 24;
        
        SDL_SetRenderDrawColor(rend, 0,0,0,255);
        SDL_RenderClear(rend);
        for (int y = 0; y < 32; ++y)
        {
            dest.y = y * 24;
            for (int x = 0; x < 32; ++x)
            {
                dest.x = x * 24;
                unsigned char* color = PALETTE[(memory[0x200 + x + y * 32] & 0xf)];
                SDL_SetRenderDrawColor(rend, color[0], color[1], color[2], color[3]);
                SDL_RenderFillRect(rend, &dest);
            }
        }
        
        for (int i = 0; i < 100; ++i)
            cpu = mosCycle(cpu);
 
        SDL_RenderPresent(rend);
 
        // calculates to 60 fps
        SDL_Delay(1000 / 60);
    }
 
    // destroy texture
    //SDL_DestroyTexture(tex);
 
    // destroy renderer
    SDL_DestroyRenderer(rend);
 
    // destroy window
    SDL_DestroyWindow(win);
     
    // close SDL
    SDL_Quit();
 
    return 0;

//end
}