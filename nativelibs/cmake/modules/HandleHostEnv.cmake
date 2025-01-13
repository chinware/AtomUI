# This source file is part of the atomui project
#
# Copyright (c) 2017 - 2022 qinware, All rights reserved.
# Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
#
# See https://qinware.com/LICENSE.txt for license information
#
# Created by chinboy on 2025/01/10.

if (WIN32)
   list(APPEND DEFAULT_DEFINES UNICODE _UNICODE _CRT_SECURE_NO_WARNINGS)
   # Windows 8 0x0602
   list(APPEND DEFAULT_DEFINES
           WINVER=0x0602 _WIN32_WINNT=0x0602
           WIN32_LEAN_AND_MEAN)
endif()

if (MSVC)
   atomui_append_flag(/Zc:__cplusplus CMAKE_CXX_FLAGS)
endif ()