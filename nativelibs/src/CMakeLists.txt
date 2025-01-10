# This source file is part of the atomui project
#
# Copyright (c) 2017 - 2022 qinware, All rights reserved.
# Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
#
# See https://qinware.com/LICENSE.txt for license information
#
# Created by chinboy on 2025/01/10.

atomui_add_library(AtomUI
    BUILD_DEFAULT
    SOURCES
    Global.cpp
    bridge/WindowUtils.cpp
    ${ATOMUI_INCLUDE_DIR}/atomui/WindowUtils.h
)

atomui_extend_library(AtomUI CONDITION APPLE
    SOURCES
    macos/WindowUtils.mm
    DEPENDS
    ${FWFoundation} ${FWAppKit}
)