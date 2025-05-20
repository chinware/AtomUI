-- This source file is part of the atomui project
--
-- Copyright (c) 2017 - 2022 qinware, All rights reserved.
-- Copyright (c) 2017 - 2022 chinboy <chinware@163.com>
--
-- See https://qinware.com/LICENSE.txt for license information
--
-- Created by chinboy on 2025/05/19.
target("AtomUINative", function()
    set_kind("shared")

    add_files("Global.cpp")
    add_files("WindowUtils.cpp")
    
    if is_os("macosx") then
        add_frameworks("Foundation", "AppKit")
        add_files("macos/WindowUtils.mm") 
    end
end)
