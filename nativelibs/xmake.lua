-- This source file is part of the atomui project
--
-- Copyright (c) 2017 - 2022 qinware, All rights reserved.
-- Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
--
-- See https://qinware.com/LICENSE.txt for license information
--
-- Created by chinboy on 2025/05/19.
--
add_rules("mode.debug", "mode.release")
set_xmakever("2.9.9")

set_project("AtomUINative")
set_version("0.0.1", {build = "%Y%m%d%H%M"})

set_warnings("all", "error")

-- set language std version
set_languages("c17", "cxx20")

-- add module directories
add_moduledirs("xmake/modules")

-- define global variables
ATOMUI_SOURCE_DIR = os.projectdir()
ATOMUI_INCLUDE_DIR = path.join(ATOMUI_SOURCE_DIR, "include/atomui")

-- add option definitions
includes("xmake/options.lua")
includes("xmake/toolchains/atomui")

add_includedirs("include", {public = true})
add_includedirs("$(builddir)/include", {public = true})

-- global config header generator
set_configdir("$(builddir)/include/atomui")
add_configfiles(path.join(ATOMUI_INCLUDE_DIR, "Config.h.in"))

ATOMUI_PACKAGE_NAME = "atomui"
ATOMUI_PACKAGE_CASED_NAME = "AtomUI"
ATOMUI_PACKAGE_STRING = ATOMUI_PACKAGE_NAME .. " " .. ATOMUI_PACKAGE_CASED_NAME
ATOMUI_PACKAGE_BUGREPORT = "https://qinware.com/bugs/"
ATOMUI_DISPLAY_NAME = ATOMUI_PACKAGE_CASED_NAME
ATOMUI_COPYRIGHT_YEAR = "2025"
ATOMUI_COMPANY_NAME = "Qinware"
ATOMUI_COPYRIGHT = "Copyright Qinware Technologies Ltd. (c) 2018-2025"

set_configvar("ATOMUI_PACKAGE_NAME", ATOMUI_PACKAGE_NAME)
set_configvar("ATOMUI_PACKAGE_CASED_NAME", ATOMUI_PACKAGE_CASED_NAME)
set_configvar("ATOMUI_PACKAGE_STRING", ATOMUI_PACKAGE_STRING)
set_configvar("ATOMUI_PACKAGE_BUGREPORT", ATOMUI_PACKAGE_BUGREPORT)
set_configvar("ATOMUI_DISPLAY_NAME", ATOMUI_DISPLAY_NAME)
set_configvar("ATOMUI_COPYRIGHT_YEAR", ATOMUI_COPYRIGHT_YEAR)
set_configvar("ATOMUI_COMPANY_NAME", ATOMUI_COMPANY_NAME)
set_configvar("ATOMUI_COPYRIGHT", ATOMUI_COPYRIGHT)

if has_config("build_unittests") then includes("tests/unittests") end

if is_mode("debug") then add_defines("ATOMUI_DEBUG_BUILD") end

includes("include/atomui")
includes("src")

before_build(function()
    import("handle_info_summary")()
end)
