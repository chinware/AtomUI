# This source file is part of the atomui project
#
# Copyright (c) 2017 - 2022 qinware, All rights reserved.
# Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
#
# See https://qinware.com/LICENSE.txt for license information
#
# Created by chinboy on 2025/01/10.

if(NOT DEFINED ATOMUI_VERSION_MAJOR)
   set(ATOMUI_VERSION_MAJOR 0)
endif()
if(NOT DEFINED ATOMUI_VERSION_MINOR)
   set(ATOMUI_VERSION_MINOR 0)
endif()
if(NOT DEFINED ATOMUI_VERSION_PATCH)
   set(ATOMUI_VERSION_PATCH 1)
endif()
if(NOT DEFINED ATOMUI_VERSION_SUFFIX)
   set(ATOMUI_VERSION_SUFFIX git)
endif()

if (NOT ATOMUI_PACKAGE_VERSION)
   set(ATOMUI_PACKAGE_VERSION "${ATOMUI_VERSION_MAJOR}.${ATOMUI_VERSION_MINOR}.${ATOMUI_VERSION_PATCH}-${ATOMUI_VERSION_SUFFIX}")
endif()

if (NOT ATOMUI_VERSION)
   set(ATOMUI_VERSION "${ATOMUI_VERSION_MAJOR}.${ATOMUI_VERSION_MINOR}.${ATOMUI_VERSION_PATCH}")
endif()

set(ATOMUI_VERSION_COMPAT ${ATOMUI_VERSION})

math(EXPR ATOMUI_VERSION_ID ${ATOMUI_VERSION_MAJOR}*10000+${ATOMUI_VERSION_MINOR}*100+${ATOMUI_VERSION_PATCH})

set(ATOMUI_COMPAT_VERSION ${ATOMUI_VERSION})
set(ATOMUI_PACKAGE_NAME atomui)
set(ATOMUI_PACKAGE_CASED_NAME AtomUI)
set(ATOMUI_PACKAGE_STRING "${ATOMUI_PACKAGE_NAME} ${ATOMUI_PACKAGE_VERSION}")
set(ATOMUI_PACKAGE_BUGREPORT "https://qinware.com/bugs/")
set(ATOMUI_DISPLAY_NAME "AtomUI")
set(ATOMUI_COPYRIGHT_YEAR "2025")
set(ATOMUI_COMPANY_NAME "Qinware")
set(ATOMUI_COPYRIGHT "Copyright Qinware Technologies Ltd. (c) 2018-2025")
