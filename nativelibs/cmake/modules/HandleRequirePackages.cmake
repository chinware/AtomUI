# This source file is part of the atomui project
#
# Copyright (c) 2017 - 2022 qinware, All rights reserved.
# Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
#
# See https://qinware.com/LICENSE.txt for license information
#
# Created by chinboy on 2025/01/10.

if (APPLE)
   find_library(FWCoreFoundation CoreFoundation)
   find_library(FWCoreServices CoreServices)
   find_library(FWFoundation Foundation)
   find_library(FWAppKit AppKit)
   find_library(FWIOKit IOKit)
   find_library(FWSecurity Security)
   find_library(FWSystemConfiguration SystemConfiguration)
   find_library(FWWebKit WebKit)
endif()