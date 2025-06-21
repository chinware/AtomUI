-- This source file is part of the atomui project
--
-- Copyright (c) 2017 - 2022 qinware, All rights reserved.
-- Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
--
-- See https://qinware.com/LICENSE.txt for license information
--
-- Created by chinboy on 2025/05/20.

local project        = import("core.project.project")
local project_config = import("core.project.config")

function main()
    cprint("--------------------------------------------------------------------------------------")
    cprint("Thank for using AtomUI project, have a lot of fun!")
    cprint("--------------------------------------------------------------------------------------")
    cprint("BUILD_MODE        : ${color.success}%s", project_config.mode())
    cprint("PROJECT_SOURCE_DIR: ${color.success}%s", os.projectdir())
    cprint("PROJECT_BINARY_DIR: ${color.success}%s", "$(builddir)")
    cprint("CONFIGURE_DIR     : ${color.success}%s", project_config.directory())
    cprint("XMAKE_INSTALL_DIR : ${color.success}%s", "$(programdir)")
    cprint("HOST              : ${color.success}%s", project_config.host())
    cprint("PLATFORM          : ${color.success}%s", project_config.plat())
    cprint("ARCHITECTURE      : ${color.success}%s", project_config.arch())
end
