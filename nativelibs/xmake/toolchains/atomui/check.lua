--!A cross-toolchain build utility based on Lua
--
-- Licensed under the Apache License, Version 2.0 (the "License");
-- you may not use this file except in compliance with the License.
-- You may obtain a copy of the License at
--
--     http://www.apache.org/licenses/LICENSE-2.0
--
-- Unless required by applicable law or agreed to in writing, software
-- distributed under the License is distributed on an "AS IS" BASIS,
-- WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
-- See the License for the specific language governing permissions and
-- limitations under the License.
--
-- Copyright (C) 2015-present, TBOOX Open Source Group.
--
-- @author      ruki
-- @file        check.lua
--

-- imports
import("core.project.config")
import("lib.detect.find_path")
import("lib.detect.find_tool")
import("detect.sdks.find_xcode")
import("detect.sdks.find_cross_toolchain")

-- find xcode on macos
function _find_xcode(toolchain)
    -- get apple device
    local simulator
    local appledev = toolchain:config("appledev") or config.get("appledev")
    if appledev and appledev == "simulator" then
        simulator = true
        appledev = "simulator"
    elseif not toolchain:is_plat("macosx") and toolchain:is_arch("i386", "x86_64") then
        simulator = true
        appledev = "simulator"
    end

    -- find xcode
    local xcode_sdkver = toolchain:config("xcode_sdkver") or config.get("xcode_sdkver")
    local xcode = find_xcode(config.get("xcode"), {force = true, verbose = true,
                                                   find_codesign = toolchain:is_global(),
                                                   sdkver = xcode_sdkver,
                                                   appledev = appledev,
                                                   plat = toolchain:plat(),
                                                   arch = toolchain:arch()})
    if not xcode then
        cprint("checking for Xcode directory ... ${color.nothing}${text.nothing}")
        return
    end

    -- save target minver
    --
    -- @note we need to differentiate the version for the system,
    -- because the xcode toolchain of iphoneos/macosx may need to be used at the same time.
    --
    -- e.g.
    --
    -- target("test")
    --     set_toolchains("xcode", {plat = os.host(), arch = os.arch()})
    --
    -- xcode found
    xcode_sdkver = xcode.sdkver
    
    local target_minver = toolchain:config("target_minver") or config.get("target_minver")
    if xcode_sdkver and not target_minver then
        target_minver = xcode.target_minver
    end

    if toolchain:is_global() then
        config.set("xcode", xcode.sdkdir, {force = true, readonly = true})
        cprint("checking for Xcode directory ... ${color.success}%s", xcode.sdkdir)
    end
    toolchain:config_set("xcode", xcode.sdkdir)
    toolchain:config_set("xcode_sdkver", xcode_sdkver)
    toolchain:config_set("target_minver", target_minver)
    toolchain:config_set("appledev", appledev)
    toolchain:configs_save()
    cprint("checking for SDK version of Xcode for %s (%s) ... ${color.success}%s", toolchain:plat(), toolchain:arch(), xcode_sdkver)
end

-- check the cross toolchain
function main(toolchain)

    -- get sdk directory
    local sdkdir = toolchain:sdkdir()
    local bindir = toolchain:bindir()
    local cross  = toolchain:cross()
    if not sdkdir and not bindir then
        bindir = try {function () return os.iorunv("llvm-config", {"--bindir"}) end}
        if bindir then
            sdkdir = path.directory(bindir)
        elseif is_host("linux") and os.isfile("/usr/bin/llvm-ar") then
            sdkdir = "/usr"
        elseif is_host("macosx") then
            if os.arch() == "arm64" then
                bindir = find_path("llvm-ar", "/opt/homebrew/opt/llvm/bin")
            else
                bindir = find_path("llvm-ar", "/usr/local/Cellar/llvm/*/bin")
            end
            if bindir then
                sdkdir = path.directory(bindir)
            end
        elseif is_host("windows") then
            local llvm_ar = find_tool("llvm-ar", {force = true, envs = {PATH = os.getenv("PATH")}})
            if llvm_ar and llvm_ar.program and path.is_absolute(llvm_ar.program) then
                bindir = path.directory(llvm_ar.program)
                sdkdir = path.directory(bindir)
            end
        end
    end

    -- find cross toolchain from external envirnoment
    local cross_toolchain = find_cross_toolchain(sdkdir, {bindir = bindir, cross = cross})
    if not cross_toolchain then
        -- find it from packages
        for _, package in ipairs(toolchain:packages()) do
            local installdir = package:installdir()
            if installdir and os.isdir(installdir) then
                cross_toolchain = find_cross_toolchain(installdir)
                if cross_toolchain then
                    break
                end
            end
        end
    end
    if cross_toolchain then
        toolchain:config_set("cross", cross_toolchain.cross)
        toolchain:config_set("bindir", cross_toolchain.bindir)
        toolchain:config_set("sdkdir", cross_toolchain.sdkdir)
        toolchain:configs_save()
    else
        raise("llvm toolchain not found!")
    end

    if toolchain:is_plat("cross") and (not toolchain:cross() or toolchain:cross():match("^%s*$")) then
        raise("Missing cross target. Use `--cross=name` to specify.")
    end

    -- attempt to find xcode to pass `-isysroot` on macos
    if toolchain:is_plat("macosx") then
        _find_xcode(toolchain)
    end
    return cross_toolchain
end
