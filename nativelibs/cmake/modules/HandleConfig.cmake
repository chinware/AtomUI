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
check_include_file(dlfcn.h HAVE_DLFCN_H)
check_include_file(errno.h HAVE_ERRNO_H)
check_include_file(fcntl.h HAVE_FCNTL_H)
check_include_file(unistd.h HAVE_UNISTD_H)
