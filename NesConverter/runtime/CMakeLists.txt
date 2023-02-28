cmake_minimum_required(VERSION 3.18)
set(CMAKE_OSX_ARCHITECTURES x86_64)
project(NativeNes CXX ASM_NASM)

set(CMAKE_CXX_STANDARD 17)

find_package(SDL2 REQUIRED)
include_directories(${SDL2_INCLUDE_DIRS})

OPTION(TITLE "Output exe title")

IF (TITLE)
    add_compile_definitions(TITLE="${TITLE}")
ENDIF (TITLE)

set(CMAKE_ASM_NASM_COMPILER yasm)
if (${CMAKE_SYSTEM_NAME} MATCHES "Darwin")
    set(CMAKE_ASM_NASM_COMPILE_OBJECT "<CMAKE_ASM_NASM_COMPILER> -f macho64 -o <OBJECT> <SOURCE>")
else()
    set(CMAKE_ASM_NASM_COMPILE_OBJECT "<CMAKE_ASM_NASM_COMPILER> -f elf64 -o <OBJECT> <SOURCE>")
endif()

add_executable(NativeNes mos6502.asm mos6502.chr.asm main.cpp NesBus.cpp NesBus.h Cartridge.cpp Cartridge.h IMemory.h CPU.cpp CPU.h PictureBus.cpp PictureBus.h PPU.h Joystick.cpp Joystick.h SimplePPU.cpp SimplePPU.h)
target_link_libraries(NativeNes ${SDL2_LIBRARIES})
