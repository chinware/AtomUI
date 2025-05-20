-- This source file is part of the atomui project
--
-- Copyright (c) 2017 - 2022 qinware, All rights reserved.
-- Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
--
-- See https://qinware.com/LICENSE.txt for license information
--
-- Created by chinboy on 2025/05/20.
local handleRequirePackages = import("handle_require_packages")
local summaryOutput = import("summary_output")
local config = import("core.project.config")

function main()
    local executed = config.get("atomui.on_config_executed")
    if executed == nil then
        handleRequirePackages()
        summaryOutput()
        config.set("atomui.on_config_executed", true)
    end
end
