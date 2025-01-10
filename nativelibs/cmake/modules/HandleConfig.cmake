# This source file is part of the atomui project
#
# Copyright (c) 2017 - 2022 qinware, All rights reserved.
# Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
#
# See https://qinware.com/LICENSE.txt for license information
#
# Created by chinboy on 2025/01/10.

if( WIN32 AND NOT CYGWIN )
   # We consider Cygwin as another Unix
   set(PURE_WINDOWS ON)
endif()

include(CheckIncludeFile)
include(CheckLibraryExists)
include(CheckSymbolExists)
include(CheckFunctionExists)

find_package(Backtrace)
set(HAVE_BACKTRACE ${Backtrace_FOUND})
set(BACKTRACE_HEADER ${Backtrace_HEADER})

check_symbol_exists(_Unwind_Backtrace "unwind.h" HAVE__UNWIND_BACKTRACE)
check_symbol_exists(sysconf unistd.h HAVE_SYSCONF)
check_symbol_exists(strerror string.h HAVE_STRERROR)
check_symbol_exists(strerror_r string.h HAVE_STRERROR_R)
check_symbol_exists(strerror_s string.h HAVE_DECL_STRERROR_S)

check_include_file(dlfcn.h HAVE_DLFCN_H)
check_include_file(errno.h HAVE_ERRNO_H)
check_include_file(fcntl.h HAVE_FCNTL_H)
check_include_file(malloc/malloc.h HAVE_MALLOC_MALLOC_H)
check_include_file(mach/mach.h HAVE_MACH_MACH_H)
check_include_file(CrashReporterClient.h HAVE_CRASHREPORTERCLIENT_H)
check_include_file(unistd.h HAVE_UNISTD_H)

if(APPLE)
   include(CheckCSourceCompiles)
   CHECK_C_SOURCE_COMPILES("
     static const char *__crashreporter_info__ = 0;
     asm(\".desc ___crashreporter_info__, 0x10\");
     int main() { return 0; }"
         HAVE_CRASHREPORTER_INFO)
endif()