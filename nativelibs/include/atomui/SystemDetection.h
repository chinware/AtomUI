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
   The operating system, must be one of: (ATOMUI_OS_x)

     DARWIN   - Any Darwin system (macOS, iOS, watchOS, tvOS)
     MACOS    - macOS
     IOS      - iOS
     WATCHOS  - watchOS
     TVOS     - tvOS
     VISIONOS - visionOS
     WIN32    - Win32 (Windows 2000/XP/Vista/7 and Windows Server 2003/2008)
     CYGWIN   - Cygwin
     SOLARIS  - Sun Solaris
     HPUX     - HP-UX
     LINUX    - Linux [has variants]
     FREEBSD  - FreeBSD [has variants]
     NETBSD   - NetBSD
     OPENBSD  - OpenBSD
     INTERIX  - Interix
     AIX      - AIX
     HURD     - GNU Hurd
     QNX      - QNX [has variants]
     QNX6     - QNX RTP 6.1
     LYNX     - LynxOS
     BSD4     - Any BSD 4.4 system
     UNIX     - Any UNIX BSD/SYSV system
     ANDROID  - Android platform
     HAIKU    - Haiku
     WEBOS    - LG WebOS
     WASM     - WebAssembly

   The following operating systems have variants:
     LINUX    - both ATOMUI_OS_LINUX and ATOMUI_OS_ANDROID are defined when building for Android
              - only ATOMUI_OS_LINUX is defined if building for other Linux systems
     MACOS    - both ATOMUI_OS_BSD4 and ATOMUI_OS_IOS are defined when building for iOS
              - both ATOMUI_OS_BSD4 and ATOMUI_OS_MACOS are defined when building for macOS
     FREEBSD  - ATOMUI_OS_FREEBSD is defined only when building for FreeBSD with a BSD userland
              - ATOMUI_OS_FREEBSD_KERNEL is always defined on FreeBSD, even if the userland is from GNU
*/

#if defined(__APPLE__) && (defined(__GNUC__) || defined(__xlC__) || defined(__xlc__))
#  include <TargetConditionals.h>
#  define ATOMUI_OS_APPLE
#  if defined(TARGET_OS_MAC) && TARGET_OS_MAC
#    define ATOMUI_OS_DARWIN
#    define ATOMUI_OS_BSD4
#    if defined(TARGET_OS_IPHONE) && TARGET_OS_IPHONE
#      define ATOMUI_PLATFORM_UIKIT
#      if defined(TARGET_OS_WATCH) && TARGET_OS_WATCH
#        define ATOMUI_OS_WATCHOS
#      elif defined(TARGET_OS_TV) && TARGET_OS_TV
#        define ATOMUI_OS_TVOS
#      elif defined(TARGET_OS_VISION) && TARGET_OS_VISION
#        define ATOMUI_OS_VISIONOS
#      else
#        // TARGET_OS_IOS is only available in newer SDKs,
#        // so assume any other iOS-based platform is iOS for now
#        define ATOMUI_OS_IOS
#      endif
#    else
#      // TARGET_OS_OSX is only available in newer SDKs,
#      // so assume any non iOS-based platform is macOS for now
#      define ATOMUI_OS_MACOS
#    endif
#  else
#    error "AtomUI has not been ported to this Apple platform
#  endif
#elif defined(__WEBOS__)
#  define ATOMUI_OS_WEBOS
#  define ATOMUI_OS_LINUX
#elif defined(__ANDROID__) || defined(ANDROID)
#  define ATOMUI_OS_ANDROID
#  define ATOMUI_OS_LINUX
#elif defined(__CYGWIN__)
#  define ATOMUI_OS_CYGWIN
#elif !defined(SAG_COM) && (!defined(WINAPI_FAMILY) || WINAPI_FAMILY==WINAPI_FAMILY_DESKTOP_APP) && (defined(WIN64) || defined(_WIN64) || defined(__WIN64__))
#  define ATOMUI_OS_WIN32
#  define ATOMUI_OS_WIN64
#elif !defined(SAG_COM) && (defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(__NT__))
#    define ATOMUI_OS_WIN32
#elif defined(__sun) || defined(sun)
#  define ATOMUI_OS_SOLARIS
#elif defined(hpux) || defined(__hpux)
#  define ATOMUI_OS_HPUX
#elif defined(__EMSCRIPTEN__)
#  define ATOMUI_OS_WASM
#elif defined(__linux__) || defined(__linux)
#  define ATOMUI_OS_LINUX
#elif defined(__FreeBSD__) || defined(__DragonFly__) || defined(__FreeBSD_kernel__)
#  ifndef __FreeBSD_kernel__
#    define ATOMUI_OS_FREEBSD
#  endif
#  define ATOMUI_OS_FREEBSD_KERNEL
#  define ATOMUI_OS_BSD4
#elif defined(__NetBSD__)
#  define ATOMUI_OS_NETBSD
#  define ATOMUI_OS_BSD4
#elif defined(__OpenBSD__)
#  define ATOMUI_OS_OPENBSD
#  define ATOMUI_OS_BSD4
#elif defined(__INTERIX)
#  define ATOMUI_OS_INTERIX
#  define ATOMUI_OS_BSD4
#elif defined(_AIX)
#  define ATOMUI_OS_AIX
#elif defined(__Lynx__)
#  define ATOMUI_OS_LYNX
#elif defined(__GNU__)
#  define ATOMUI_OS_HURD
#elif defined(__QNXNTO__)
#  define ATOMUI_OS_QNX
#elif defined(__INTEGRITY)
#  define ATOMUI_OS_INTEGRITY
#elif defined(__rtems__)
#  define ATOMUI_OS_RTEMS
#elif defined(__vxworks)
#  define ATOMUI_OS_VXWORKS
#elif defined(__HAIKU__)
#  define ATOMUI_OS_HAIKU
#elif defined(__MAKEDEPEND__)
#else
#  error "AtomUI has not been ported to this OS
#endif

#if defined(ATOMUI_OS_WIN32) || defined(ATOMUI_OS_WIN64)
#  define ATOMUI_OS_WINDOWS
#  define ATOMUI_OS_WIN
// On Windows, pointers to dllimport'ed variables are not constant expressions,
// so to keep to certain initializations (like QMetaObject) constexpr, we need
// to use functions instead.
#  define ATOMUI_NO_DATA_RELOCATION
#endif

#if defined(ATOMUI_OS_WIN)
#  undef ATOMUI_OS_UNIX
#elif !defined(ATOMUI_OS_UNIX)
#  define ATOMUI_OS_UNIX
#endif

#ifdef ATOMUI_OS_DARWIN
#  include <Availability.h>
#  include <AvailabilityMacros.h>

#  define ATOMUI_DARWIN_PLATFORM_SDK_EQUAL_OR_ABOVE(macos, ios, tvos, watchos) \
    ((defined(__MAC_OS_X_VERSION_MAX_ALLOWED) && macos != __MAC_NA && __MAC_OS_X_VERSION_MAX_ALLOWED >= macos) || \
     (defined(__IPHONE_OS_VERSION_MAX_ALLOWED) && ios != __IPHONE_NA && __IPHONE_OS_VERSION_MAX_ALLOWED >= ios) || \
     (defined(__TV_OS_VERSION_MAX_ALLOWED) && tvos != __TVOS_NA && __TV_OS_VERSION_MAX_ALLOWED >= tvos) || \
     (defined(__WATCH_OS_VERSION_MAX_ALLOWED) && watchos != __WATCHOS_NA && __WATCH_OS_VERSION_MAX_ALLOWED >= watchos))

#  define ATOMUI_DARWIN_DEPLOYMENT_TARGET_BELOW(macos, ios, tvos, watchos) \
    ((defined(__MAC_OS_X_VERSION_MIN_REQUIRED) && macos != __MAC_NA && __MAC_OS_X_VERSION_MIN_REQUIRED < macos) || \
     (defined(__IPHONE_OS_VERSION_MIN_REQUIRED) && ios != __IPHONE_NA && __IPHONE_OS_VERSION_MIN_REQUIRED < ios) || \
     (defined(__TV_OS_VERSION_MIN_REQUIRED) && tvos != __TVOS_NA && __TV_OS_VERSION_MIN_REQUIRED < tvos) || \
     (defined(__WATCH_OS_VERSION_MIN_REQUIRED) && watchos != __WATCHOS_NA && __WATCH_OS_VERSION_MIN_REQUIRED < watchos))

#  define ATOMUI_MACOS_IOS_PLATFORM_SDK_EQUAL_OR_ABOVE(macos, ios) \
      ATOMUI_DARWIN_PLATFORM_SDK_EQUAL_OR_ABOVE(macos, ios, __TVOS_NA, __WATCHOS_NA)
#  define ATOMUI_MACOS_PLATFORM_SDK_EQUAL_OR_ABOVE(macos) \
      ATOMUI_DARWIN_PLATFORM_SDK_EQUAL_OR_ABOVE(macos, __IPHONE_NA, __TVOS_NA, __WATCHOS_NA)
#  define ATOMUI_IOS_PLATFORM_SDK_EQUAL_OR_ABOVE(ios) \
      ATOMUI_DARWIN_PLATFORM_SDK_EQUAL_OR_ABOVE(__MAC_NA, ios, __TVOS_NA, __WATCHOS_NA)
#  define ATOMUI_TVOS_PLATFORM_SDK_EQUAL_OR_ABOVE(tvos) \
      ATOMUI_DARWIN_PLATFORM_SDK_EQUAL_OR_ABOVE(__MAC_NA, __IPHONE_NA, tvos, __WATCHOS_NA)
#  define ATOMUI_WATCHOS_PLATFORM_SDK_EQUAL_OR_ABOVE(watchos) \
      ATOMUI_DARWIN_PLATFORM_SDK_EQUAL_OR_ABOVE(__MAC_NA, __IPHONE_NA, __TVOS_NA, watchos)

#  define ATOMUI_MACOS_IOS_DEPLOYMENT_TARGET_BELOW(macos, ios) \
      ATOMUI_DARWIN_DEPLOYMENT_TARGET_BELOW(macos, ios, __TVOS_NA, __WATCHOS_NA)
#  define ATOMUI_MACOS_DEPLOYMENT_TARGET_BELOW(macos) \
      ATOMUI_DARWIN_DEPLOYMENT_TARGET_BELOW(macos, __IPHONE_NA, __TVOS_NA, __WATCHOS_NA)
#  define ATOMUI_IOS_DEPLOYMENT_TARGET_BELOW(ios) \
      ATOMUI_DARWIN_DEPLOYMENT_TARGET_BELOW(__MAC_NA, ios, __TVOS_NA, __WATCHOS_NA)
#  define ATOMUI_TVOS_DEPLOYMENT_TARGET_BELOW(tvos) \
      ATOMUI_DARWIN_DEPLOYMENT_TARGET_BELOW(__MAC_NA, __IPHONE_NA, tvos, __WATCHOS_NA)
#  define ATOMUI_WATCHOS_DEPLOYMENT_TARGET_BELOW(watchos) \
      ATOMUI_DARWIN_DEPLOYMENT_TARGET_BELOW(__MAC_NA, __IPHONE_NA, __TVOS_NA, watchos)

#else // !ATOMUI_OS_DARWIN

#define ATOMUI_DARWIN_PLATFORM_SDK_EQUAL_OR_ABOVE(macos, ios, tvos, watchos) (0)
#define ATOMUI_MACOS_IOS_PLATFORM_SDK_EQUAL_OR_ABOVE(macos, ios) (0)
#define ATOMUI_MACOS_PLATFORM_SDK_EQUAL_OR_ABOVE(macos) (0)
#define ATOMUI_IOS_PLATFORM_SDK_EQUAL_OR_ABOVE(ios) (0)
#define ATOMUI_TVOS_PLATFORM_SDK_EQUAL_OR_ABOVE(tvos) (0)
#define ATOMUI_WATCHOS_PLATFORM_SDK_EQUAL_OR_ABOVE(watchos) (0)

#endif // ATOMUI_OS_DARWIN

#ifdef __LSB_VERSION__
#  if __LSB_VERSION__ < 40
#    error "This version of the Linux Standard Base is unsupported"
#  endif
#ifndef ATOMUI_LINUXBASE
#  define ATOMUI_LINUXBASE
#endif
#endif

#if defined (__ELF__)
#  define ATOMUI_OF_ELF
#endif
#if defined (__MACH__) && defined (__APPLE__)
#  define ATOMUI_OF_MACH_O
#endif

#if defined(__SIZEOF_INT128__)
// Compiler used in VxWorks SDK declares __SIZEOF_INT128__ but VxWorks doesn't support this type,
// so we can't rely solely on compiler here.
// MSVC STL used by MSVC and clang-cl does not support int128
#if !defined(ATOMUI_OS_VXWORKS) && !defined(_MSC_VER)
#  define ATOMUI_COMPILER_SUPPORTS_INT128 __SIZEOF_INT128__
#endif
#endif // defined(__SIZEOF_INT128__)