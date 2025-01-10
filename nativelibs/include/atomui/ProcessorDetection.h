// This source file is part of the atomui project
//
// Copyright (c) 2017 - 2025 qinware, All rights reserved.
// Copyright (c) 2017 - 2025 chinboy <chinware@163.com>
//
// See https://qinware.com/LICENSE.txt for license information
//
// Created by chinboy on 2025/01/10.

#pragma once

/*
    This file uses preprocessor #defines to set various ATOMUI_PROCESSOR_* #defines
    based on the following patterns:

    ATOMUI_PROCESSOR_{FAMILY}
    ATOMUI_PROCESSOR_{FAMILY}_{VARIANT}
    ATOMUI_PROCESSOR_{FAMILY}_{REVISION}

    The first is always defined. Defines for the various revisions/variants are
    optional and usually dependent on how the compiler was invoked. Variants
    that are a superset of another should have a define for the superset.

    In addition to the processor family, variants, and revisions, we also set
    ATOMUI_BYTE_ORDER appropriately for the target processor. For bi-endian
    processors, we try to auto-detect the byte order using the __BIG_ENDIAN__,
    __LITTLE_ENDIAN__, or __BYTE_ORDER__ preprocessor macros.

    Note: when adding support for new processors, be sure to update
    config.tests/arch/arch.cpp to ensure that configure can detect the target
    and host architectures.
*/

/* Machine byte-order, reuse preprocessor provided macros when available */
#if defined(__ORDER_BIG_ENDIAN__)
#  define ATOMUI_BIG_ENDIAN __ORDER_BIG_ENDIAN__
#else
#  define ATOMUI_BIG_ENDIAN 4321
#endif
#if defined(__ORDER_LITTLE_ENDIAN__)
#  define ATOMUI_LITTLE_ENDIAN __ORDER_LITTLE_ENDIAN__
#else
#  define ATOMUI_LITTLE_ENDIAN 1234
#endif

/*
    Alpha family, no revisions or variants

    Alpha is bi-endian, use endianness auto-detection implemented below.
*/
#if defined(__alpha__) || defined(_M_ALPHA)
#  define ATOMUI_PROCESSOR_ALPHA
// ATOMUI_BYTE_ORDER not defined, use endianness auto-detection

/*
    ARM family, known revisions: V5, V6, V7, V8

    ARM is bi-endian, detect using __ARMEL__ or __ARMEB__, falling back to
    auto-detection implemented below.
*/
#elif defined(__arm__) || defined(__TARGET_ARCH_ARM) || defined(_M_ARM) || defined(_M_ARM64) || defined(__aarch64__) || defined(__ARM64__)
#  if defined(__aarch64__) || defined(__ARM64__) || defined(_M_ARM64)
#    define ATOMUI_PROCESSOR_ARM_64
#    define ATOMUI_PROCESSOR_WORDSIZE 8
#  else
#    define ATOMUI_PROCESSOR_ARM_32
#  endif
#  if defined(__ARM_ARCH) && __ARM_ARCH > 1
#    define ATOMUI_PROCESSOR_ARM __ARM_ARCH
#  elif defined(__TARGET_ARCH_ARM) && __TARGET_ARCH_ARM > 1
#    define ATOMUI_PROCESSOR_ARM __TARGET_ARCH_ARM
#  elif defined(_M_ARM) && _M_ARM > 1
#    define ATOMUI_PROCESSOR_ARM _M_ARM
#  elif defined(__ARM64_ARCH_8__) \
      || defined(__aarch64__) \
      || defined(__ARMv8__) \
      || defined(__ARMv8_A__) \
      || defined(_M_ARM64)
#    define ATOMUI_PROCESSOR_ARM 8
#  elif defined(__ARM_ARCH_7__) \
      || defined(__ARM_ARCH_7A__) \
      || defined(__ARM_ARCH_7R__) \
      || defined(__ARM_ARCH_7M__) \
      || defined(__ARM_ARCH_7S__) \
      || defined(_ARM_ARCH_7) \
      || defined(__CORE_CORTEXA__)
#    define ATOMUI_PROCESSOR_ARM 7
#  elif defined(__ARM_ARCH_6__) \
      || defined(__ARM_ARCH_6J__) \
      || defined(__ARM_ARCH_6T2__) \
      || defined(__ARM_ARCH_6Z__) \
      || defined(__ARM_ARCH_6K__) \
      || defined(__ARM_ARCH_6ZK__) \
      || defined(__ARM_ARCH_6M__)
#    define ATOMUI_PROCESSOR_ARM 6
#  elif defined(__ARM_ARCH_5TEJ__) \
        || defined(__ARM_ARCH_5TE__)
#    define ATOMUI_PROCESSOR_ARM 5
#  else
#    define ATOMUI_PROCESSOR_ARM 0
#  endif
#  if ATOMUI_PROCESSOR_ARM >= 8
#    define ATOMUI_PROCESSOR_ARM_V8
#  endif
#  if ATOMUI_PROCESSOR_ARM >= 7
#    define ATOMUI_PROCESSOR_ARM_V7
#  endif
#  if ATOMUI_PROCESSOR_ARM >= 6
#    define ATOMUI_PROCESSOR_ARM_V6
#  endif
#  if ATOMUI_PROCESSOR_ARM >= 5
#    define ATOMUI_PROCESSOR_ARM_V5
#  else
#    error "ARM architecture too old"
#  endif
#  if defined(__ARMEL__) || defined(_M_ARM64)
#    define ATOMUI_BYTE_ORDER ATOMUI_LITTLE_ENDIAN
#  elif defined(__ARMEB__)
#    define ATOMUI_BYTE_ORDER ATOMUI_BIG_ENDIAN
#  else
// ATOMUI_BYTE_ORDER not defined, use endianness auto-detection
#endif

/*
    AVR32 family, no revisions or variants

    AVR32 is big-endian.
*/
// #elif defined(__avr32__)
// #  define ATOMUI_PROCESSOR_AVR32
// #  define ATOMUI_BYTE_ORDER ATOMUI_BIG_ENDIAN

/*
    Blackfin family, no revisions or variants

    Blackfin is little-endian.
*/
// #elif defined(__bfin__)
// #  define ATOMUI_PROCESSOR_BLACKFIN
// #  define ATOMUI_BYTE_ORDER ATOMUI_LITTLE_ENDIAN

/*
    PA-RISC family, no revisions or variants

    PA-RISC is big-endian.
*/
#elif defined(__hppa__)
#  define ATOMUI_PROCESSOR_HPPA
#  define ATOMUI_BYTE_ORDER ATOMUI_BIG_ENDIAN

/*
    X86 family, known variants: 32- and 64-bit

    X86 is little-endian.
*/
#elif defined(__i386) || defined(__i386__) || defined(_M_IX86)
#  define ATOMUI_PROCESSOR_X86_32
#  define ATOMUI_BYTE_ORDER ATOMUI_LITTLE_ENDIAN
#  define ATOMUI_PROCESSOR_WORDSIZE   4

/*
 * We define ATOMUI_PROCESSOR_X86 == 6 for anything above a equivalent or better
 * than a Pentium Pro (the processor whose architecture was called P6) or an
 * Athlon.
 *
 * All processors since the Pentium III and the Athlon 4 have SSE support, so
 * we use that to detect. That leaves the original Athlon, Pentium Pro and
 * Pentium II.
 */

#  if defined(_M_IX86)
#    define ATOMUI_PROCESSOR_X86     (_M_IX86/100)
#  elif defined(__i686__) || defined(__athlon__) || defined(__SSE__) || defined(__pentiumpro__)
#    define ATOMUI_PROCESSOR_X86     6
#  elif defined(__i586__) || defined(__k6__) || defined(__pentium__)
#    define ATOMUI_PROCESSOR_X86     5
#  elif defined(__i486__) || defined(__80486__)
#    define ATOMUI_PROCESSOR_X86     4
#  else
#    define ATOMUI_PROCESSOR_X86     3
#  endif

#elif defined(__x86_64) || defined(__x86_64__) || defined(__amd64) || defined(_M_X64)
#  define ATOMUI_PROCESSOR_X86       6
#  define ATOMUI_PROCESSOR_X86_64
#  define ATOMUI_BYTE_ORDER ATOMUI_LITTLE_ENDIAN
#  define ATOMUI_PROCESSOR_WORDSIZE   8

/*
    Itanium (IA-64) family, no revisions or variants

    Itanium is bi-endian, use endianness auto-detection implemented below.
*/
#elif defined(__ia64) || defined(__ia64__) || defined(_M_IA64)
#  define ATOMUI_PROCESSOR_IA64
#  define ATOMUI_PROCESSOR_WORDSIZE   8
// ATOMUI_BYTE_ORDER not defined, use endianness auto-detection

/*
    LoongArch family, known variants: 32- and 64-bit

    LoongArch is little-endian.
*/
#elif defined(__loongarch__)
#  define ATOMUI_PROCESSOR_LOONGARCH
#  if __loongarch_grlen == 64
#    define ATOMUI_PROCESSOR_LOONGARCH_64
#  else
#    define ATOMUI_PROCESSOR_LOONGARCH_32
#  endif
#  define ATOMUI_BYTE_ORDER ATOMUI_LITTLE_ENDIAN

/*
    Motorola 68000 family, no revisions or variants

    M68K is big-endian.
*/
#elif defined(__m68k__)
#  define ATOMUI_PROCESSOR_M68K
#  define ATOMUI_BYTE_ORDER ATOMUI_BIG_ENDIAN

/*
    MIPS family, known revisions: I, II, III, IV, 32, 64

    MIPS is bi-endian, use endianness auto-detection implemented below.
*/
#elif defined(__mips) || defined(__mips__) || defined(_M_MRX000)
#  define ATOMUI_PROCESSOR_MIPS
#  if defined(_MIPS_ARCH_MIPS1) || (defined(__mips) && __mips - 0 >= 1)
#    define ATOMUI_PROCESSOR_MIPS_I
#  endif
#  if defined(_MIPS_ARCH_MIPS2) || (defined(__mips) && __mips - 0 >= 2)
#    define ATOMUI_PROCESSOR_MIPS_II
#  endif
#  if defined(_MIPS_ARCH_MIPS3) || (defined(__mips) && __mips - 0 >= 3)
#    define ATOMUI_PROCESSOR_MIPS_III
#  endif
#  if defined(_MIPS_ARCH_MIPS4) || (defined(__mips) && __mips - 0 >= 4)
#    define ATOMUI_PROCESSOR_MIPS_IV
#  endif
#  if defined(_MIPS_ARCH_MIPS5) || (defined(__mips) && __mips - 0 >= 5)
#    define ATOMUI_PROCESSOR_MIPS_V
#  endif
#  if defined(_MIPS_ARCH_MIPS32) || defined(__mips32) || (defined(__mips) && __mips - 0 >= 32)
#    define ATOMUI_PROCESSOR_MIPS_32
#  endif
#  if defined(_MIPS_ARCH_MIPS64) || defined(__mips64)
#    define ATOMUI_PROCESSOR_MIPS_64
#    define ATOMUI_PROCESSOR_WORDSIZE 8
#  endif
#  if defined(__MIPSEL__)
#    define ATOMUI_BYTE_ORDER ATOMUI_LITTLE_ENDIAN
#  elif defined(__MIPSEB__)
#    define ATOMUI_BYTE_ORDER ATOMUI_BIG_ENDIAN
#  else
// ATOMUI_BYTE_ORDER not defined, use endianness auto-detection
#  endif

/*
    Power family, known variants: 32- and 64-bit

    There are many more known variants/revisions that we do not handle/detect.
    See http://en.wikipedia.org/wiki/Power_Architecture
    and http://en.wikipedia.org/wiki/File:PowerISA-evolution.svg

    Power is bi-endian, use endianness auto-detection implemented below.
*/
#elif defined(__ppc__) || defined(__ppc) || defined(__powerpc__) \
      || defined(_ARCH_COM) || defined(_ARCH_PWR) || defined(_ARCH_PPC)  \
      || defined(_M_MPPC) || defined(_M_PPC)
#  define ATOMUI_PROCESSOR_POWER
#  if defined(__ppc64__) || defined(__powerpc64__) || defined(__64BIT__)
#    define ATOMUI_PROCESSOR_POWER_64
#    define ATOMUI_PROCESSOR_WORDSIZE 8
#  else
#    define ATOMUI_PROCESSOR_POWER_32
#  endif
// ATOMUI_BYTE_ORDER not defined, use endianness auto-detection

/*
    RISC-V family, known variants: 32- and 64-bit

    RISC-V is little-endian.
*/
#elif defined(__riscv)
#  define ATOMUI_PROCESSOR_RISCV
#  if __riscv_xlen == 64
#    define ATOMUI_PROCESSOR_RISCV_64
#  else
#    define ATOMUI_PROCESSOR_RISCV_32
#  endif
#  define ATOMUI_BYTE_ORDER ATOMUI_LITTLE_ENDIAN

/*
    S390 family, known variant: S390X (64-bit)

    S390 is big-endian.
*/
#elif defined(__s390__)
#  define ATOMUI_PROCESSOR_S390
#  if defined(__s390x__)
#    define ATOMUI_PROCESSOR_S390_X
#  endif
#  define ATOMUI_BYTE_ORDER ATOMUI_BIG_ENDIAN

/*
    SuperH family, optional revision: SH-4A

    SuperH is bi-endian, use endianness auto-detection implemented below.
*/
// #elif defined(__sh__)
// #  define ATOMUI_PROCESSOR_SH
// #  if defined(__sh4a__)
// #    define ATOMUI_PROCESSOR_SH_4A
// #  endif
// ATOMUI_BYTE_ORDER not defined, use endianness auto-detection

/*
    SPARC family, optional revision: V9

    SPARC is big-endian only prior to V9, while V9 is bi-endian with big-endian
    as the default byte order. Assume all SPARC systems are big-endian.
*/
#elif defined(__sparc__)
#  define ATOMUI_PROCESSOR_SPARC
#  if defined(__sparc_v9__) || defined(__sparcv9)
#    define ATOMUI_PROCESSOR_SPARC_V9
#  endif
#  if defined(__sparc64__)
#    define ATOMUI_PROCESSOR_SPARC_64
#  endif
#  define ATOMUI_BYTE_ORDER ATOMUI_BIG_ENDIAN

#endif

/*
  NOTE:
  GCC 4.6 added __BYTE_ORDER__, __ORDER_BIG_ENDIAN__, __ORDER_LITTLE_ENDIAN__
  and __ORDER_PDP_ENDIAN__ in SVN r165881. If you are using GCC 4.6 or newer,
  this code will properly detect your target byte order; if you are not, and
  the __LITTLE_ENDIAN__ or __BIG_ENDIAN__ macros are not defined, then this
  code will fail to detect the target byte order.
*/
// Some processors support either endian format, try to detect which we are using.
#if !defined(ATOMUI_BYTE_ORDER)
#  if defined(__BYTE_ORDER__) && (__BYTE_ORDER__ == ATOMUI_BIG_ENDIAN || __BYTE_ORDER__ == ATOMUI_LITTLE_ENDIAN)
// Reuse __BYTE_ORDER__ as-is, since our ATOMUI_*_ENDIAN #defines match the preprocessor defaults
#    define ATOMUI_BYTE_ORDER __BYTE_ORDER__
#  elif defined(__BIG_ENDIAN__) || defined(_big_endian__) || defined(_BIG_ENDIAN)
#    define ATOMUI_BYTE_ORDER ATOMUI_BIG_ENDIAN
#  elif defined(__LITTLE_ENDIAN__) || defined(_little_endian__) || defined(_LITTLE_ENDIAN)
#    define ATOMUI_BYTE_ORDER ATOMUI_LITTLE_ENDIAN
#  else
#    error "Unable to determine byte order!"
#  endif
#endif

/*
   Size of a pointer and the machine register size. We detect a 64-bit system by:
   * GCC and compatible compilers (Clang, ICC on OS X and Windows) always define
     __SIZEOF_POINTER__. This catches all known cases of ILP32 builds on 64-bit
     processors.
   * Most other Unix compilers define __LP64__ or _LP64 on 64-bit mode
     (Long and Pointer 64-bit)
   * If ATOMUI_PROCESSOR_WORDSIZE was defined above, it's assumed to match the pointer
     size.
   Otherwise, we assume to be 32-bit and then check in qglobal.cpp that it is right.
*/

#if defined __SIZEOF_POINTER__
#  define ATOMUI_POINTER_SIZE           __SIZEOF_POINTER__
#elif defined(__LP64__) || defined(_LP64)
#  define ATOMUI_POINTER_SIZE           8
#elif defined(ATOMUI_PROCESSOR_WORDSIZE)
#  define ATOMUI_POINTER_SIZE           ATOMUI_PROCESSOR_WORDSIZE
#else
#  define ATOMUI_POINTER_SIZE           4
#endif

/*
   Define ATOMUI_PROCESSOR_WORDSIZE to be the size of the machine's word (usually,
   the size of the register). On some architectures where a pointer could be
   smaller than the register, the macro is defined above.

   Falls back to ATOMUI_POINTER_SIZE if not set explicitly for the platform.
*/
#ifndef ATOMUI_PROCESSOR_WORDSIZE
#  define ATOMUI_PROCESSOR_WORDSIZE        ATOMUI_POINTER_SIZE
#endif
