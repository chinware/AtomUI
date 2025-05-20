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

set_project("AtomUINativeLibs")
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
includes("xmake/meta_info.lua")

add_includedirs("include", {public = true})
add_includedirs("$(buildir)/include", {public = true})

set_configdir("$(buildir)/include/atomui")

if has_config("build_unittests") then includes("tests/unittests") end

if is_mode("debug") then add_defines("ATOMUI_DEBUG_BUILD") end

includes("include/atomui")
includes("src")

before_build(function()
    import("handle_on_config")()
end)
