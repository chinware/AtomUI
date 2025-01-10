# This source file is part of the atomui project
#
# Copyright (c) 2017 - 2022 qinware, All rights reserved.
# Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
#
# See https://qinware.com/LICENSE.txt for license information
#
# Created by chinboy on 2025/01/10.

function(set_if_arch_bitness var_name)
   cmake_parse_arguments(
         SIA # prefix
         "" # options
         "ARCH;CASE_32_BIT;CASE_64_BIT" # single-value args
         "" # multi-value args
         ${ARGN})

   if("${SIA_ARCH}" STREQUAL "i386" OR
         "${SIA_ARCH}" STREQUAL "i686" OR
         "${SIA_ARCH}" STREQUAL "x86" OR
         "${SIA_ARCH}" STREQUAL "armv5" OR
         "${SIA_ARCH}" STREQUAL "armv6" OR
         "${SIA_ARCH}" STREQUAL "armv7" OR
         "${SIA_ARCH}" STREQUAL "armv7k" OR
         "${SIA_ARCH}" STREQUAL "arm64_32" OR
         "${SIA_ARCH}" STREQUAL "armv7s" OR
         "${SIA_ARCH}" STREQUAL "wasm32" OR
         "${SIA_ARCH}" STREQUAL "powerpc")
      set("${var_name}" "${SIA_CASE_32_BIT}" PARENT_SCOPE)
   elseif("${SIA_ARCH}" STREQUAL "x86_64" OR
         "${SIA_ARCH}" STREQUAL "amd64" OR
         "${SIA_ARCH}" STREQUAL "arm64" OR
         "${SIA_ARCH}" STREQUAL "arm64e" OR
         "${SIA_ARCH}" STREQUAL "aarch64" OR
         "${SIA_ARCH}" STREQUAL "powerpc64" OR
         "${SIA_ARCH}" STREQUAL "powerpc64le" OR
         "${SIA_ARCH}" STREQUAL "s390x")
      set("${var_name}" "${SIA_CASE_64_BIT}" PARENT_SCOPE)
   else()
      message(FATAL_ERROR "Unknown architecture: ${SIA_ARCH}")
   endif()
endfunction()