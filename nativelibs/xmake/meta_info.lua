-- This source file is part of the atomui project
--
-- Copyright (c) 2017 - 2022 qinware, All rights reserved.
-- Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
--
-- See https://qinware.com/LICENSE.txt for license information
--
-- Created by chinboy on 2025/05/19.
ATOMUI_PACKAGE_NAME = "atomui"
ATOMUI_PACKAGE_CASED_NAME = "AtomUI"
ATOMUI_PACKAGE_STRING = ATOMUI_PACKAGE_NAME .. " " .. ATOMUI_PACKAGE_CASED_NAME
ATOMUI_PACKAGE_BUGREPORT = "https://qinware.com/bugs/"
ATOMUI_DISPLAY_NAME = ATOMUI_PACKAGE_CASED_NAME
ATOMUI_COPYRIGHT_YEAR = "2025"
ATOMUI_COMPANY_NAME = "Qinware"
ATOMUI_COPYRIGHT = "Copyright Qinware Technologies Ltd. (c) 2018-2025"

add_configfiles(path.join(ATOMUI_INCLUDE_DIR, "Config.h.in"))
set_configvar("ATOMUI_PACKAGE_NAME", ATOMUI_PACKAGE_NAME)
set_configvar("ATOMUI_PACKAGE_CASED_NAME", ATOMUI_PACKAGE_CASED_NAME)
set_configvar("ATOMUI_PACKAGE_STRING", ATOMUI_PACKAGE_STRING)
set_configvar("ATOMUI_PACKAGE_BUGREPORT", ATOMUI_PACKAGE_BUGREPORT)
set_configvar("ATOMUI_DISPLAY_NAME", ATOMUI_DISPLAY_NAME)
set_configvar("ATOMUI_COPYRIGHT_YEAR", ATOMUI_COPYRIGHT_YEAR)
set_configvar("ATOMUI_COMPANY_NAME", ATOMUI_COMPANY_NAME)
set_configvar("ATOMUI_COPYRIGHT", ATOMUI_COPYRIGHT)

-- target("config_summary", function()
--     set_kind("phony")
--     on_config(function()
--         import("handle_on_config")()
--     end)
-- end)
