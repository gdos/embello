# gcc Makefile for LPC810
# based on original file by Kamal Mostafa <kamal@whence.com>

VPATH = $(SHARED)

CROSS = arm-none-eabi-
CPU = -mthumb -mcpu=cortex-m0plus
WARN = -Wall
STD = -std=gnu99

CC = $(CROSS)gcc
CXX = $(CROSS)g++
LD = $(CROSS)ld
OBJCOPY = $(CROSS)objcopy
SIZE = $(CROSS)size

CFLAGS += $(CPU) $(WARN) $(STD) -MMD -I$(SHARED) -DIRQ_DISABLE \
          -Os -ffunction-sections -fno-builtin -ggdb
CXXFLAGS += $(CPU) $(WARN) -MMD -I$(SHARED) -DIRQ_DISABLE \
          -Os -ffunction-sections -fno-builtin -ggdb
CXXFLAGS += -fno-rtti -fno-exceptions

LDFLAGS += --gc-sections -Map=firmware.map --cref --library-path=$(SHARED)
LIBGCC = $(shell $(CC) $(CFLAGS) --print-libgcc-file-name)

OS := $(shell uname)

ifeq ($(OS), Linux)
TTY ?= /dev/ttyUSB*
endif

ifeq ($(OS), Darwin)
TTY ?= /dev/tty.usbserial-*
endif

.PHONY: all clean isp
  
all: firmware.bin

firmware.elf: $(SHARED)/$(LINK) $(OBJS)
	@$(LD) -o $@ $(LDFLAGS) -T $(SHARED)/$(LINK) $(OBJS) $(LIBGCC)
	$(SIZE) $@

clean:
	rm -f *.o *.d firmware.elf firmware.bin firmware.map

# this works with NXP LPC's, using serial ISP
isp: firmware.bin
	lpc21isp $(ISPOPTS) -control -bin firmware.bin $(TTY) 115200 0

%.bin:%.elf
#	@$(OBJCOPY) --strip-unneeded -O ihex firmware.elf firmware.hex
	@$(OBJCOPY) --strip-unneeded -O binary firmware.elf firmware.bin

-include $(OBJS:.o=.d)
