// This source file is part of the atomui project
//
// Copyright (c) 2017 - 2025 qinware, All rights reserved.
// Copyright (c) 2017 - 2025 chinboy <chinware@163.com>
//
// See https://qinware.com/LICENSE.txt for license information
//
// Created by chinboy on 2025/01/10.

#pragma once

#include "atomui/SystemDetection.h"
#include "atomui/ProcessorDetection.h"

/*
   The compiler, must be one of: (ATOMUI_CC_x)

     COVERITY - Coverity cov-scan
     SYM      - Digital Mars C/C++ (used to be Symantec C++)
     MSVC     - Microsoft Visual C/C++, Intel C++ for Windows
     BOR      - Borland/Turbo C++
     WAT      - Watcom C++
     GNU      - GNU C++
     COMEAU   - Comeau C++
     EDG      - Edison Design Group C++
     OC       - CenterLine C++
     SUN      - Forte Developer, or Sun Studio C++
     MIPS     - MIPSpro C++
     DEC      - DEC C++
     HPACC    - HP aC++
     USLC     - SCO OUDK and UDK
     CDS      - Reliant C++
     KAI      - KAI C++
     INTEL    - Intel C++ for Linux, Intel C++ for Windows
     HIGHC    - MetaWare High C/C++
     PGI      - Portland Group C++
     GHS      - Green Hills Optimizing C++ Compilers
     RVCT     - ARM Realview Compiler Suite
     CLANG    - C++ front-end for the LLVM compiler


   Should be sorted most to least authoritative.
*/

#if defined(__COVERITY__)
#  define ATOMUI_CC_COVERITY
#  define ATOMUI_COMPILER_COMPLAINS_ABOUT_RETURN_AFTER_UNREACHABLE
#endif

/* Symantec C++ is now Digital Mars */
#if defined(__DMC__) || defined(__SC__)
#  define ATOMUI_CC_SYM
/* "explicit" semantics implemented in 8.1e but keyword recognized since 7.5 */
#  if defined(__SC__) && __SC__ < 0x750
#    error "Compiler not supported"
#  endif

#elif defined(_MSC_VER)
#  define ATOMUI_CC_MSVC (_MSC_VER)
#  define ATOMUI_CC_MSVC_NET
#  define ATOMUI_CC_MSVC_ONLY ATOMUI_CC_MSVC
#  ifdef __clang__
#    undef ATOMUI_CC_MSVC_ONLY
#    define ATOMUI_CC_CLANG ((__clang_major__ * 100) + __clang_minor__)
#    define ATOMUI_CC_CLANG_ONLY ATOMUI_CC_CLANG
#  endif
#  define ATOMUI_OUTOFLINE_TEMPLATE inline
#  define ATOMUI_COMPILER_MANGLES_RETURN_TYPE
#  define ATOMUI_COMPILER_MANGLES_ACCESS_SPECIFIER
#  define ATOMUI_FUNC_INFO __FUNCSIG__
#  define ATOMUI_ASSUME_IMPL(expr) __assume(expr)
#  define ATOMUI_UNREACHABLE_IMPL() __assume(0)
#  define ATOMUI_DECL_EXPORT __declspec(dllexport)
#  define ATOMUI_DECL_IMPORT __declspec(dllimport)
#  if _MSC_VER < 1938 // stdext is deprecated since VS 2022 17.8
#    define ATOMUI_MAKE_CHECKED_ARRAY_ITERATOR(x, N) stdext::make_checked_array_iterator(x, size_t(N)) // Since _MSC_VER >= 1500
#  endif
#  define ATOMUI_COMPILER_COMPLAINS_ABOUT_RETURN_AFTER_UNREACHABLE

#elif defined(__BORLANDC__) || defined(__TURBOC__)
#  define ATOMUI_CC_BOR
#  define ATOMUI_INLINE_TEMPLATE
#  if __BORLANDC__ < 0x502
#    error "Compiler not supported"
#  endif

#elif defined(__WATCOMC__)
#  define ATOMUI_CC_WAT

/* ARM Realview Compiler Suite
   RVCT compiler also defines __EDG__ and __GNUC__ (if --gnu flag is given),
   so check for it before that */
#elif defined(__ARMCC__) || defined(__CC_ARM)
#  define ATOMUI_CC_RVCT
/* work-around for missing compiler intrinsics */
#  define __is_empty(X) false
#  define __is_pod(X) false
#  define ATOMUI_DECL_DEPRECATED __attribute__ ((__deprecated__))
#  ifdef ATOMUI_OS_LINUX
#    define ATOMUI_DECL_EXPORT     __attribute__((visibility("default")))
#    define ATOMUI_DECL_IMPORT     __attribute__((visibility("default")))
#    define ATOMUI_DECL_HIDDEN     __attribute__((visibility("hidden")))
#  else
#    define ATOMUI_DECL_EXPORT     __declspec(dllexport)
#    define ATOMUI_DECL_IMPORT     __declspec(dllimport)
#  endif

#elif defined(__GNUC__)
#  define ATOMUI_CC_GNU          (__GNUC__ * 100 + __GNUC_MINOR__)
#  if defined(__MINGW32__)
#    define ATOMUI_CC_MINGW
#  endif
#  if defined(__clang__)
/* Clang also masquerades as GCC */
#    if defined(__apple_build_version__)
      // The Clang version reported by Apple Clang in __clang_major__
      // and __clang_minor__ does _not_ reflect the actual upstream
      // version of the compiler. To allow consumers to use a single
      // define to verify the Clang version we hard-code the versions
      // based on the best available info we have about the actual
      // version: http://en.wikipedia.org/wiki/Xcode#Toolchain_Versions
#      if __apple_build_version__   >= 14030022 // Xcode 14.3
#        define ATOMUI_CC_CLANG 1500
#      elif __apple_build_version__ >= 14000029 // Xcode 14.0
#        define ATOMUI_CC_CLANG 1400
#      elif __apple_build_version__ >= 13160021 // Xcode 13.3
#        define ATOMUI_CC_CLANG 1300
#      elif __apple_build_version__ >= 13000029 // Xcode 13.0
#        define ATOMUI_CC_CLANG 1200
#      elif __apple_build_version__ >= 12050022 // Xcode 12.5
#        define ATOMUI_CC_CLANG 1110
#      elif __apple_build_version__ >= 12000032 // Xcode 12.0
#        define ATOMUI_CC_CLANG 1000
#      elif __apple_build_version__ >= 11030032 // Xcode 11.4
#        define ATOMUI_CC_CLANG 900
#      elif __apple_build_version__ >= 11000033 // Xcode 11.0
#        define ATOMUI_CC_CLANG 800
#      else
#        error "Unsupported Apple Clang version"
#      endif
#    else
       // Non-Apple Clang, so we trust the versions reported
#      define ATOMUI_CC_CLANG ((__clang_major__ * 100) + __clang_minor__)
#    endif
#    define ATOMUI_CC_CLANG_ONLY ATOMUI_CC_CLANG
#    if __has_builtin(__builtin_assume)
#      define ATOMUI_ASSUME_IMPL(expr)   __builtin_assume(expr)
#    else
#      define ATOMUI_ASSUME_IMPL(expr)  if (expr){} else __builtin_unreachable()
#    endif
#    define ATOMUI_UNREACHABLE_IMPL() __builtin_unreachable()
#    if !defined(__has_extension)
#      /* Compatibility with older Clang versions */
#      define __has_extension __has_feature
#    endif
#    if defined(__APPLE__)
     /* Apple/clang specific features */
#      define ATOMUI_DECL_CF_RETURNS_RETAINED __attribute__((cf_returns_retained))
#      ifdef __OBJC__
#        define ATOMUI_DECL_NS_RETURNS_AUTORELEASED __attribute__((ns_returns_autoreleased))
#      endif
#    endif
#    ifdef __EMSCRIPTEN__
#      define ATOMUI_CC_EMSCRIPTEN
#    endif
#  else
/* Plain GCC */
#    define ATOMUI_CC_GNU_ONLY ATOMUI_CC_GNU
#    if ATOMUI_CC_GNU >= 405
#      define ATOMUI_ASSUME_IMPL(expr)  if (expr){} else __builtin_unreachable()
#      define ATOMUI_UNREACHABLE_IMPL() __builtin_unreachable()
#      define ATOMUI_DECL_DEPRECATED_X(text) __attribute__ ((__deprecated__(text)))
#    endif
#  endif

#  ifdef ATOMUI_OS_WIN
#    define ATOMUI_DECL_EXPORT     __declspec(dllexport)
#    define ATOMUI_DECL_IMPORT     __declspec(dllimport)
#  else
#    define ATOMUI_DECL_EXPORT_OVERRIDABLE __attribute__((visibility("default"), weak))
#    ifdef ATOMUI_USE_PROTECTED_VISIBILITY
#      define ATOMUI_DECL_EXPORT     __attribute__((visibility("protected")))
#    else
#      define ATOMUI_DECL_EXPORT     __attribute__((visibility("default")))
#    endif
#    define ATOMUI_DECL_IMPORT     __attribute__((visibility("default")))
#    define ATOMUI_DECL_HIDDEN     __attribute__((visibility("hidden")))
#  endif

#  define ATOMUI_FUNC_INFO       __PRETTY_FUNCTION__
#  define ATOMUI_TYPEOF(expr)    __typeof__(expr)
#  define ATOMUI_DECL_DEPRECATED __attribute__ ((__deprecated__))
#  define ATOMUI_DECL_UNUSED     __attribute__((__unused__))
#  define ATOMUI_LIKELY(expr)    __builtin_expect(!!(expr), true)
#  define ATOMUI_UNLIKELY(expr)  __builtin_expect(!!(expr), false)
#  define ATOMUI_NORETURN        __attribute__((__noreturn__))
#  define ATOMUI_REQUIRED_RESULT __attribute__ ((__warn_unused_result__))
#  define ATOMUI_DECL_PURE_FUNCTION __attribute__((pure))
#  define ATOMUI_DECL_CONST_FUNCTION __attribute__((const))
#  define ATOMUI_DECL_COLD_FUNCTION __attribute__((cold))
#  if ATOMUI_CC_GNU >= 403 && !defined(ATOMUI_CC_CLANG)
#      define ATOMUI_ALLOC_SIZE(x) __attribute__((alloc_size(x)))
#  endif

/* IBM compiler versions are a bit messy. There are actually two products:
   the C product, and the C++ product. The C++ compiler is always packaged
   with the latest version of the C compiler. Version numbers do not always
   match. This little table (I'm not sure it's accurate) should be helpful:

   C++ product                C product

   C Set 3.1                  C Compiler 3.0
   ...                        ...
   C++ Compiler 3.6.6         C Compiler 4.3
   ...                        ...
   Visual Age C++ 4.0         ...
   ...                        ...
   Visual Age C++ 5.0         C Compiler 5.0
   ...                        ...
   Visual Age C++ 6.0         C Compiler 6.0

   Now:
   __xlC__    is the version of the C compiler in hexadecimal notation
              is only an approximation of the C++ compiler version
   __IBMCPP__ is the version of the C++ compiler in decimal notation
              but it is not defined on older compilers like C Set 3.1 */
#elif defined(__xlC__)
#  define ATOMUI_CC_XLC
#  if __xlC__ < 0x400
#    error "Compiler not supported"
#  elif __xlC__ >= 0x0600
#    define ATOMUI_TYPEOF(expr)      __typeof__(expr)
#    define ATOMUI_PACKED            __attribute__((__packed__))
#  endif

/* Older versions of DEC C++ do not define __EDG__ or __EDG - observed
   on DEC C++ V5.5-004. New versions do define  __EDG__ - observed on
   Compaq C++ V6.3-002.
   This compiler is different enough from other EDG compilers to handle
   it separately anyway. */
#elif defined(__DECCXX) || defined(__DECC)
#  define ATOMUI_CC_DEC
/* Compaq C++ V6 compilers are EDG-based but I'm not sure about older
   DEC C++ V5 compilers. */
#  if defined(__EDG__)
#    define ATOMUI_CC_EDG
#  endif
/* Compaq has disabled EDG's _BOOL macro and uses _BOOL_EXISTS instead
   - observed on Compaq C++ V6.3-002.
   In any case versions prior to Compaq C++ V6.0-005 do not have bool. */
#  if !defined(_BOOL_EXISTS)
#    error "Compiler not supported"
#  endif
/* Spurious (?) error messages observed on Compaq C++ V6.5-014. */
/* Apply to all versions prior to Compaq C++ V6.0-000 - observed on
   DEC C++ V5.5-004. */
#  if __DECCXX_VER < 60060000
#    define ATOMUI_BROKEN_TEMPLATE_SPECIALIZATION
#  endif
/* avoid undefined symbol problems with out-of-line template members */
#  define ATOMUI_OUTOFLINE_TEMPLATE inline

/* The Portland Group C++ compiler is based on EDG and does define __EDG__
   but the C compiler does not */
#elif defined(__PGI)
#  define ATOMUI_CC_PGI
#  if defined(__EDG__)
#    define ATOMUI_CC_EDG
#  endif

/* Compilers with EDG front end are similar. To detect them we test:
   __EDG documented by SGI, observed on MIPSpro 7.3.1.1 and KAI C++ 4.0b
   __EDG__ documented in EDG online docs, observed on Compaq C++ V6.3-002
   and PGI C++ 5.2-4 */
#elif !defined(ATOMUI_OS_HPUX) && (defined(__EDG) || defined(__EDG__))
#  define ATOMUI_CC_EDG
/* From the EDG documentation (does not seem to apply to Compaq C++ or GHS C):
   _BOOL
        Defined in C++ mode when bool is a keyword. The name of this
        predefined macro is specified by a configuration flag. _BOOL
        is the default.
   __BOOL_DEFINED
        Defined in Microsoft C++ mode when bool is a keyword. */
#  if !defined(_BOOL) && !defined(__BOOL_DEFINED) && !defined(__ghs)
#    error "Compiler not supported"
#  endif

/* The Comeau compiler is based on EDG and does define __EDG__ */
#  if defined(__COMO__)
#    define ATOMUI_CC_COMEAU

/* The `using' keyword was introduced to avoid KAI C++ warnings
   but it's now causing KAI C++ errors instead. The standard is
   unclear about the use of this keyword, and in practice every
   compiler is using its own set of rules. Forget it. */
#  elif defined(__KCC)
#    define ATOMUI_CC_KAI

/* Uses CFront, make sure to read the manual how to tweak templates. */
#  elif defined(__ghs)
#    define ATOMUI_CC_GHS
#    define ATOMUI_DECL_DEPRECATED __attribute__ ((__deprecated__))
#    define ATOMUI_PACKED __attribute__ ((__packed__))
#    define ATOMUI_FUNC_INFO       __PRETTY_FUNCTION__
#    define ATOMUI_TYPEOF(expr)      __typeof__(expr)
#    define ATOMUI_UNREACHABLE_IMPL()
#    if defined(__cplusplus)
#      define ATOMUI_COMPILER_AUTO_TYPE
#      define ATOMUI_COMPILER_STATIC_ASSERT
#      define ATOMUI_COMPILER_RANGE_FOR
#      if __GHS_VERSION_NUMBER >= 201505
#        define ATOMUI_COMPILER_ALIGNAS
#        define ATOMUI_COMPILER_ALIGNOF
#        define ATOMUI_COMPILER_ATOMICS
#        define ATOMUI_COMPILER_ATTRIBUTES
#        define ATOMUI_COMPILER_AUTO_FUNCTION
#        define ATOMUI_COMPILER_CLASS_ENUM
#        define ATOMUI_COMPILER_DECLTYPE
#        define ATOMUI_COMPILER_DEFAULT_MEMBERS
#        define ATOMUI_COMPILER_DELETE_MEMBERS
#        define ATOMUI_COMPILER_DELEGATING_CONSTRUCTORS
#        define ATOMUI_COMPILER_EXPLICIT_CONVERSIONS
#        define ATOMUI_COMPILER_EXPLICIT_OVERRIDES
#        define ATOMUI_COMPILER_EXTERN_TEMPLATES
#        define ATOMUI_COMPILER_INHERITING_CONSTRUCTORS
#        define ATOMUI_COMPILER_INITIALIZER_LISTS
#        define ATOMUI_COMPILER_LAMBDA
#        define ATOMUI_COMPILER_NONSTATIC_MEMBER_INIT
#        define ATOMUI_COMPILER_NOEXCEPT
#        define ATOMUI_COMPILER_NULLPTR
#        define ATOMUI_COMPILER_RANGE_FOR
#        define ATOMUI_COMPILER_RAW_STRINGS
#        define ATOMUI_COMPILER_REF_QUALIFIERS
#        define ATOMUI_COMPILER_RVALUE_REFS
#        define ATOMUI_COMPILER_STATIC_ASSERT
#        define ATOMUI_COMPILER_TEMPLATE_ALIAS
#        define ATOMUI_COMPILER_THREAD_LOCAL
#        define ATOMUI_COMPILER_UDL
#        define ATOMUI_COMPILER_UNICODE_STRINGS
#        define ATOMUI_COMPILER_UNIFORM_INIT
#        define ATOMUI_COMPILER_UNRESTRICTED_UNIONS
#        define ATOMUI_COMPILER_VARIADIC_MACROS
#        define ATOMUI_COMPILER_VARIADIC_TEMPLATES
#      endif
#    endif //__cplusplus

#  elif defined(__DCC__)
#    define ATOMUI_CC_DIAB
#    if !defined(__bool)
#      error "Compiler not supported"
#    endif

/* The UnixWare 7 UDK compiler is based on EDG and does define __EDG__ */
#  elif defined(__USLC__) && defined(__SCO_VERSION__)
#    define ATOMUI_CC_USLC
/* The latest UDK 7.1.1b does not need this, but previous versions do */
#    if !defined(__SCO_VERSION__) || (__SCO_VERSION__ < 302200010)
#      define ATOMUI_OUTOFLINE_TEMPLATE inline
#    endif

/* Never tested! */
#  elif defined(CENTERLINE_CLPP) || defined(OBJECTCENTER)
#    define ATOMUI_CC_OC

/* CDS++ defines __EDG__ although this is not documented in the Reliant
   documentation. It also follows conventions like _BOOL and this documented */
#  elif defined(sinix)
#    define ATOMUI_CC_CDS
#  endif

/* VxWorks' DIAB toolchain has an additional EDG type C++ compiler
   (see __DCC__ above). This one is for C mode files (__EDG is not defined) */
#elif defined(_DIAB_TOOL)
#  define ATOMUI_CC_DIAB
#  define ATOMUI_FUNC_INFO       __PRETTY_FUNCTION__

/* Never tested! */
#elif defined(__HIGHC__)
#  define ATOMUI_CC_HIGHC

#elif defined(__SUNPRO_CC) || defined(__SUNPRO_C)
#  define ATOMUI_CC_SUN
#  define ATOMUI_COMPILER_MANGLES_RETURN_TYPE
/* 5.0 compiler or better
    'bool' is enabled by default but can be disabled using -features=nobool
    in which case _BOOL is not defined
        this is the default in 4.2 compatibility mode triggered by -compat=4 */
#  if __SUNPRO_CC >= 0x500
#    define ATOMUI_NO_TEMPLATE_TEMPLATE_PARAMETERS
   /* see http://developers.sun.com/sunstudio/support/Ccompare.html */
#    if __SUNPRO_CC >= 0x590
#      define ATOMUI_TYPEOF(expr)    __typeof__(expr)
#    endif
#    if __SUNPRO_CC >= 0x550
#      define ATOMUI_DECL_EXPORT     __global
#    endif
#    if !defined(_BOOL)
#      error "Compiler not supported"
#    endif
/* 4.2 compiler or older */
#  else
#    error "Compiler not supported"
#  endif

/* CDS++ does not seem to define __EDG__ or __EDG according to Reliant
   documentation but nevertheless uses EDG conventions like _BOOL */
#elif defined(sinix)
#  define ATOMUI_CC_EDG
#  define ATOMUI_CC_CDS
#  if !defined(_BOOL)
#    error "Compiler not supported"
#  endif
#  define ATOMUI_BROKEN_TEMPLATE_SPECIALIZATION

#else
#  error "AtomUI has not been tested with this compiler
#endif

/*
 * SG10's SD-6 feature detection and some useful extensions from Clang and GCC
 * https://isocpp.org/std/standing-documents/sd-6-sg10-feature-test-recommendations
 * http://clang.llvm.org/docs/LanguageExtensions.html#feature-checking-macros
 * Not using wrapper macros, per http://eel.is/c++draft/cpp.cond#7.sentence-2
 */
#ifndef __has_builtin
#  define __has_builtin(x)             0
#endif
#ifndef __has_feature
#  define __has_feature(x)             0
#endif
#ifndef __has_attribute
#  define __has_attribute(x)           0
#endif
#ifndef __has_cpp_attribute
#  define __has_cpp_attribute(x)       0
#endif
#ifndef __has_include
#  define __has_include(x)             0
#endif
#ifndef __has_include_next
#  define __has_include_next(x)        0
#endif

/*
   detecting ASAN can be helpful to disable slow tests
   clang uses feature, gcc  defines __SANITIZE_ADDRESS__
   unconditionally check both in case other compilers mirror
   either of those options
 */
#if __has_feature(address_sanitizer) || defined(__SANITIZE_ADDRESS__)
#  define ATOMUI_ASAN_ENABLED
#endif

#ifdef __cplusplus
# if __has_include(<version>) /* remove this check once Integrity, QNX have caught up */
#  include <version>
# endif
#endif

/*
 * C++11 support
 *
 *  Paper           Macro                               SD-6 macro
 *  N2341           ATOMUI_COMPILER_ALIGNAS
 *  N2341           ATOMUI_COMPILER_ALIGNOF
 *  N2427           ATOMUI_COMPILER_ATOMICS
 *  N2761           ATOMUI_COMPILER_ATTRIBUTES               __cpp_attributes = 200809
 *  N2541           ATOMUI_COMPILER_AUTO_FUNCTION
 *  N1984 N2546     ATOMUI_COMPILER_AUTO_TYPE
 *  N2437           ATOMUI_COMPILER_CLASS_ENUM
 *  N2235           ATOMUI_COMPILER_CONSTEXPR                __cpp_constexpr = 200704
 *  N2343 N3276     ATOMUI_COMPILER_DECLTYPE                 __cpp_decltype = 200707
 *  N2346           ATOMUI_COMPILER_DEFAULT_MEMBERS
 *  N2346           ATOMUI_COMPILER_DELETE_MEMBERS
 *  N1986           ATOMUI_COMPILER_DELEGATING_CONSTRUCTORS
 *  N2437           ATOMUI_COMPILER_EXPLICIT_CONVERSIONS
 *  N3206 N3272     ATOMUI_COMPILER_EXPLICIT_OVERRIDES
 *  N1987           ATOMUI_COMPILER_EXTERN_TEMPLATES
 *  N2540           ATOMUI_COMPILER_INHERITING_CONSTRUCTORS
 *  N2672           ATOMUI_COMPILER_INITIALIZER_LISTS
 *  N2658 N2927     ATOMUI_COMPILER_LAMBDA                   __cpp_lambdas = 200907
 *  N2756           ATOMUI_COMPILER_NONSTATIC_MEMBER_INIT
 *  N2855 N3050     ATOMUI_COMPILER_NOEXCEPT
 *  N2431           ATOMUI_COMPILER_NULLPTR
 *  N2930           ATOMUI_COMPILER_RANGE_FOR
 *  N2442           ATOMUI_COMPILER_RAW_STRINGS              __cpp_raw_strings = 200710
 *  N2439           ATOMUI_COMPILER_REF_QUALIFIERS
 *  N2118 N2844 N3053 ATOMUI_COMPILER_RVALUE_REFS            __cpp_rvalue_references = 200610
 *  N1720           ATOMUI_COMPILER_STATIC_ASSERT            __cpp_static_assert = 200410
 *  N2258           ATOMUI_COMPILER_TEMPLATE_ALIAS
 *  N2659           ATOMUI_COMPILER_THREAD_LOCAL
 *  N2660           ATOMUI_COMPILER_THREADSAFE_STATICS
 *  N2765           ATOMUI_COMPILER_UDL                      __cpp_user_defined_literals = 200809
 *  N2442           ATOMUI_COMPILER_UNICODE_STRINGS          __cpp_unicode_literals = 200710
 *  N2640           ATOMUI_COMPILER_UNIFORM_INIT
 *  N2544           ATOMUI_COMPILER_UNRESTRICTED_UNIONS
 *  N1653           ATOMUI_COMPILER_VARIADIC_MACROS
 *  N2242 N2555     ATOMUI_COMPILER_VARIADIC_TEMPLATES       __cpp_variadic_templates = 200704
 *
 *
 * For the C++ standards C++14 and C++17, we use only the SD-6 macro.
 *
 * For any future version of the C++ standard, we use only the C++20 feature test macro.
 * For library features, we assume <version> is present (this header includes it).
 *
 * For a full listing of feature test macros, see
 *  https://en.cppreference.com/w/cpp/feature_test
 * Exceptions:
 *  ATOMUI_DECL_CONSTEXPR_DTOR           constexpr in C++20 for explicit destructors __cpp_constexpr >= 201907L
 *  ATOMUI_CONSTEXPR_DTOR                constexpr in C++20 for variables __cpp_constexpr >= 201907L otherwise const
 *  ATOMUI_DECL_EATOMUI_DELETE_X(message)     = delete("reason"), __cpp_deleted_function >= 202403L
 *
 * C++ extensions:
 *  ATOMUI_COMPILER_RESTRICTED_VLA       variable-length arrays, prior to __cpp_runtime_arrays
 */

/*
 * Now that we require C++17, we unconditionally expect threadsafe statics mandated since C++11
 */
#define ATOMUI_COMPILER_THREADSAFE_STATICS

#if defined(ATOMUI_CC_CLANG)
/* General C++ features */
#  define ATOMUI_COMPILER_RESTRICTED_VLA
#  if __has_feature(attribute_deprecated_with_message)
#    define ATOMUI_DECL_DEPRECATED_X(text) __attribute__ ((__deprecated__(text)))
#  endif

// Clang supports binary literals in C, C++98 and C++11 modes
// It's been supported "since the dawn of time itself" (cf. commit 179883)
#  if __has_extension(cxx_binary_literals)
#    define ATOMUI_COMPILER_BINARY_LITERALS
#  endif

// Variadic macros are supported for gnu++98, c++11, c99 ... since 2.9
#  if ATOMUI_CC_CLANG >= 209
#    if !defined(__STRICT_ANSI__) || defined(__GXX_EXPERIMENTAL_CXX0X__) \
      || (defined(__cplusplus) && (__cplusplus >= 201103L)) \
      || (defined(__STDC_VERSION__) && (__STDC_VERSION__ >= 199901L))
#      define ATOMUI_COMPILER_VARIADIC_MACROS
#    endif
#  endif

/* C++11 features, see http://clang.llvm.org/cxx_status.html */
#  if (defined(__cplusplus) && __cplusplus >= 201103L) \
      || defined(__GXX_EXPERIMENTAL_CXX0X__)
    /* Detect C++ features using __has_feature(), see http://clang.llvm.org/docs/LanguageExtensions.html#cxx11 */
#    if __has_feature(cxx_alignas)
#      define ATOMUI_COMPILER_ALIGNAS
#      define ATOMUI_COMPILER_ALIGNOF
#    endif
#    if __has_feature(cxx_atomic) && __has_include(<atomic>)
#     define ATOMUI_COMPILER_ATOMICS
#    endif
#    if __has_feature(cxx_attributes)
#      define ATOMUI_COMPILER_ATTRIBUTES
#    endif
#    if __has_feature(cxx_auto_type)
#      define ATOMUI_COMPILER_AUTO_FUNCTION
#      define ATOMUI_COMPILER_AUTO_TYPE
#    endif
#    if __has_feature(cxx_strong_enums)
#      define ATOMUI_COMPILER_CLASS_ENUM
#    endif
#    if __has_feature(cxx_constexpr) && ATOMUI_CC_CLANG > 302 /* CLANG 3.2 has bad/partial support */
#      define ATOMUI_COMPILER_CONSTEXPR
#    endif
#    if __has_feature(cxx_decltype) /* && __has_feature(cxx_decltype_incomplete_return_types) */
#      define ATOMUI_COMPILER_DECLTYPE
#    endif
#    if __has_feature(cxx_defaulted_functions)
#      define ATOMUI_COMPILER_DEFAULT_MEMBERS
#    endif
#    if __has_feature(cxx_deleted_functions)
#      define ATOMUI_COMPILER_DELETE_MEMBERS
#    endif
#    if __has_feature(cxx_delegating_constructors)
#      define ATOMUI_COMPILER_DELEGATING_CONSTRUCTORS
#    endif
#    if __has_feature(cxx_explicit_conversions)
#      define ATOMUI_COMPILER_EXPLICIT_CONVERSIONS
#    endif
#    if __has_feature(cxx_override_control)
#      define ATOMUI_COMPILER_EXPLICIT_OVERRIDES
#    endif
#    if __has_feature(cxx_inheriting_constructors)
#      define ATOMUI_COMPILER_INHERITING_CONSTRUCTORS
#    endif
#    if __has_feature(cxx_generalized_initializers)
#      define ATOMUI_COMPILER_INITIALIZER_LISTS
#      define ATOMUI_COMPILER_UNIFORM_INIT /* both covered by this feature macro, according to docs */
#    endif
#    if __has_feature(cxx_lambdas)
#      define ATOMUI_COMPILER_LAMBDA
#    endif
#    if __has_feature(cxx_noexcept)
#      define ATOMUI_COMPILER_NOEXCEPT
#    endif
#    if __has_feature(cxx_nonstatic_member_init)
#      define ATOMUI_COMPILER_NONSTATIC_MEMBER_INIT
#    endif
#    if __has_feature(cxx_nullptr)
#      define ATOMUI_COMPILER_NULLPTR
#    endif
#    if __has_feature(cxx_range_for)
#      define ATOMUI_COMPILER_RANGE_FOR
#    endif
#    if __has_feature(cxx_raw_string_literals)
#      define ATOMUI_COMPILER_RAW_STRINGS
#    endif
#    if __has_feature(cxx_reference_qualified_functions)
#      define ATOMUI_COMPILER_REF_QUALIFIERS
#    endif
#    if __has_feature(cxx_rvalue_references)
#      define ATOMUI_COMPILER_RVALUE_REFS
#    endif
#    if __has_feature(cxx_static_assert)
#      define ATOMUI_COMPILER_STATIC_ASSERT
#    endif
#    if __has_feature(cxx_alias_templates)
#      define ATOMUI_COMPILER_TEMPLATE_ALIAS
#    endif
#    if __has_feature(cxx_thread_local)
#      if !defined(__FreeBSD__) /* FreeBSD clang fails on __cxa_thread_atexit */
#        define ATOMUI_COMPILER_THREAD_LOCAL
#      endif
#    endif
#    if __has_feature(cxx_user_literals)
#      define ATOMUI_COMPILER_UDL
#    endif
#    if __has_feature(cxx_unicode_literals)
#      define ATOMUI_COMPILER_UNICODE_STRINGS
#    endif
#    if __has_feature(cxx_unrestricted_unions)
#      define ATOMUI_COMPILER_UNRESTRICTED_UNIONS
#    endif
#    if __has_feature(cxx_variadic_templates)
#      define ATOMUI_COMPILER_VARIADIC_TEMPLATES
#    endif
    /* Features that have no __has_feature() check */
#    if ATOMUI_CC_CLANG >= 209 /* since clang 2.9 */
#      define ATOMUI_COMPILER_EXTERN_TEMPLATES
#    endif
#  endif // (defined(__cplusplus) && __cplusplus >= 201103L) || defined(__GXX_EXPERIMENTAL_CXX0X__)

/* C++1y features, deprecated macros. Do not update this list. */
#  if defined(__cplusplus) && __cplusplus > 201103L
//#    if __has_feature(cxx_binary_literals)
//#      define ATOMUI_COMPILER_BINARY_LITERALS  // see above
//#    endif
#    if __has_feature(cxx_generic_lambda)
#      define ATOMUI_COMPILER_GENERIC_LAMBDA
#    endif
#    if __has_feature(cxx_init_capture)
#      define ATOMUI_COMPILER_LAMBDA_CAPTURES
#    endif
#    if __has_feature(cxx_relaxed_constexpr)
#      define ATOMUI_COMPILER_RELAXED_CONSTEXPR_FUNCTIONS
#    endif
#    if __has_feature(cxx_decltype_auto) && __has_feature(cxx_return_type_deduction)
#      define ATOMUI_COMPILER_RETURN_TYPE_DEDUCTION
#    endif
#    if __has_feature(cxx_variable_templates)
#      define ATOMUI_COMPILER_VARIABLE_TEMPLATES
#    endif
#    if __has_feature(cxx_runtime_array)
#      define ATOMUI_COMPILER_VLA
#    endif
#  endif // if defined(__cplusplus) && __cplusplus > 201103L

#  if defined(__STDC_VERSION__)
#    if __has_feature(c_static_assert)
#      define ATOMUI_COMPILER_STATIC_ASSERT
#    endif
#    if __has_feature(c_thread_local) && __has_include(<threads.h>)
#      if !defined(__FreeBSD__) /* FreeBSD clang fails on __cxa_thread_atexit */
#        define ATOMUI_COMPILER_THREAD_LOCAL
#      endif
#    endif
#  endif

#  ifndef ATOMUI_DECL_UNUSED
#    define ATOMUI_DECL_UNUSED __attribute__((__unused__))
#  endif
#  define ATOMUI_DECL_UNUSED_MEMBER ATOMUI_DECL_UNUSED
#endif //  defined(ATOMUI_CC_CLANG)

#if defined(ATOMUI_CC_GNU_ONLY)
#  define ATOMUI_COMPILER_RESTRICTED_VLA
#  if ATOMUI_CC_GNU >= 403
//   GCC supports binary literals in C, C++98 and C++11 modes
#    define ATOMUI_COMPILER_BINARY_LITERALS
#  endif
#  if !defined(__STRICT_ANSI__) || defined(__GXX_EXPERIMENTAL_CXX0X__) \
    || (defined(__cplusplus) && (__cplusplus >= 201103L)) \
    || (defined(__STDC_VERSION__) && (__STDC_VERSION__ >= 199901L))
     // Variadic macros are supported for gnu++98, c++11, C99 ... since forever (gcc 2.97)
#    define ATOMUI_COMPILER_VARIADIC_MACROS
#  endif
#  if defined(__GXX_EXPERIMENTAL_CXX0X__) || __cplusplus >= 201103L
#    if ATOMUI_CC_GNU >= 403
       /* C++11 features supported in GCC 4.3: */
#      define ATOMUI_COMPILER_DECLTYPE
#      define ATOMUI_COMPILER_RVALUE_REFS
#      define ATOMUI_COMPILER_STATIC_ASSERT
#    endif
#    if ATOMUI_CC_GNU >= 404
       /* C++11 features supported in GCC 4.4: */
#      define ATOMUI_COMPILER_AUTO_FUNCTION
#      define ATOMUI_COMPILER_AUTO_TYPE
#      define ATOMUI_COMPILER_EXTERN_TEMPLATES
#      define ATOMUI_COMPILER_UNIFORM_INIT
#      define ATOMUI_COMPILER_UNICODE_STRINGS
#      define ATOMUI_COMPILER_VARIADIC_TEMPLATES
#    endif
#    if ATOMUI_CC_GNU >= 405
       /* C++11 features supported in GCC 4.5: */
#      define ATOMUI_COMPILER_EXPLICIT_CONVERSIONS
       /* GCC 4.4 implements initializer_list but does not define typedefs required
        * by the standard. */
#      define ATOMUI_COMPILER_INITIALIZER_LISTS
#      define ATOMUI_COMPILER_LAMBDA
#      define ATOMUI_COMPILER_RAW_STRINGS
#      define ATOMUI_COMPILER_CLASS_ENUM
#    endif
#    if ATOMUI_CC_GNU >= 406
       /* Pre-4.6 compilers implement a non-final snapshot of N2346, hence default and delete
        * functions are supported only if they are public. Starting from 4.6, GCC handles
        * final version - the access modifier is not relevant. */
#      define ATOMUI_COMPILER_DEFAULT_MEMBERS
#      define ATOMUI_COMPILER_DELETE_MEMBERS
       /* C++11 features supported in GCC 4.6: */
#      define ATOMUI_COMPILER_NULLPTR
#      define ATOMUI_COMPILER_UNRESTRICTED_UNIONS
#      define ATOMUI_COMPILER_RANGE_FOR
#    endif
#    if ATOMUI_CC_GNU >= 407
       /* GCC 4.4 implemented <atomic> and std::atomic using its old intrinsics.
        * However, the implementation is incomplete for most platforms until GCC 4.7:
        * instead, std::atomic would use an external lock. Since we need an std::atomic
        * that is behavior-compatible with QBasicAtomic, we only enable it here */
#      define ATOMUI_COMPILER_ATOMICS
       /* GCC 4.6.x has problems dealing with noexcept expressions,
        * so turn the feature on for 4.7 and above, only */
#      define ATOMUI_COMPILER_NOEXCEPT
       /* C++11 features supported in GCC 4.7: */
#      define ATOMUI_COMPILER_NONSTATIC_MEMBER_INIT
#      define ATOMUI_COMPILER_DELEGATING_CONSTRUCTORS
#      define ATOMUI_COMPILER_EXPLICIT_OVERRIDES
#      define ATOMUI_COMPILER_TEMPLATE_ALIAS
#      define ATOMUI_COMPILER_UDL
#    endif
#    if ATOMUI_CC_GNU >= 408
#      define ATOMUI_COMPILER_ATTRIBUTES
#      define ATOMUI_COMPILER_ALIGNAS
#      define ATOMUI_COMPILER_ALIGNOF
#      define ATOMUI_COMPILER_INHERITING_CONSTRUCTORS
#      define ATOMUI_COMPILER_THREAD_LOCAL
#      if ATOMUI_CC_GNU > 408 || __GNUC_PATCHLEVEL__ >= 1
#         define ATOMUI_COMPILER_REF_QUALIFIERS
#      endif
#    endif
#    if ATOMUI_CC_GNU >= 500
       /* GCC 4.6 introduces constexpr, but it's bugged (at least) in the whole
        * 4.x series, see e.g. https://gcc.gnu.org/bugzilla/show_bug.cgi?id=57694 */
#      define ATOMUI_COMPILER_CONSTEXPR
#    endif
#  endif
#  if __cplusplus > 201103L
#    if ATOMUI_CC_GNU >= 409
     /* C++1y features in GCC 4.9 - deprecated, do not update this list */
//#    define ATOMUI_COMPILER_BINARY_LITERALS   // already supported since GCC 4.3 as an extension
#      define ATOMUI_COMPILER_LAMBDA_CAPTURES
#      define ATOMUI_COMPILER_RETURN_TYPE_DEDUCTION
#    endif
#  endif
#  if defined(__STDC_VERSION__) && __STDC_VERSION__ > 199901L
#    if ATOMUI_CC_GNU >= 407
       /* C11 features supported in GCC 4.7: */
#      define ATOMUI_COMPILER_STATIC_ASSERT
#    endif
#    if ATOMUI_CC_GNU >= 409 && defined(__has_include)
       /* C11 features supported in GCC 4.9: */
#      if __has_include(<threads.h>)
#        define ATOMUI_COMPILER_THREAD_LOCAL
#      endif
#    endif
#  endif
#endif

#if defined(ATOMUI_CC_MSVC) && !defined(ATOMUI_CC_CLANG)
#  if defined(__cplusplus)
       /* C++11 features supported in VC8 = VC2005: */
#      define ATOMUI_COMPILER_VARIADIC_MACROS

       /* 2005 supports the override and final contextual keywords, in
        the same positions as the C++11 variants, but 'final' is
        called 'sealed' instead:
        http://msdn.microsoft.com/en-us/library/0w2w91tf%28v=vs.80%29.aspx
        The behavior is slightly different in C++/CLI, which requires the
        "virtual" keyword to be present too, so don't define for that.
        So don't define ATOMUI_COMPILER_EXPLICIT_OVERRIDES (since it's not
        the same as the C++11 version), but define the ATOMUI_DECL_* flags
        accordingly. */
       /* C++11 features supported in VC10 = VC2010: */
#      define ATOMUI_COMPILER_AUTO_FUNCTION
#      define ATOMUI_COMPILER_AUTO_TYPE
#      define ATOMUI_COMPILER_DECLTYPE
#      define ATOMUI_COMPILER_EXTERN_TEMPLATES
#      define ATOMUI_COMPILER_LAMBDA
#      define ATOMUI_COMPILER_NULLPTR
#      define ATOMUI_COMPILER_RVALUE_REFS
#      define ATOMUI_COMPILER_STATIC_ASSERT
       /* C++11 features supported in VC11 = VC2012: */
#      define ATOMUI_COMPILER_EXPLICIT_OVERRIDES /* ...and use std C++11 now   */
#      define ATOMUI_COMPILER_CLASS_ENUM
#      define ATOMUI_COMPILER_ATOMICS
       /* C++11 features in VC12 = VC2013 */
#      define ATOMUI_COMPILER_DELETE_MEMBERS
#      define ATOMUI_COMPILER_DELEGATING_CONSTRUCTORS
#      define ATOMUI_COMPILER_EXPLICIT_CONVERSIONS
#      define ATOMUI_COMPILER_NONSTATIC_MEMBER_INIT
#      define ATOMUI_COMPILER_RAW_STRINGS
#      define ATOMUI_COMPILER_TEMPLATE_ALIAS
#      define ATOMUI_COMPILER_VARIADIC_TEMPLATES
#      define ATOMUI_COMPILER_INITIALIZER_LISTS // VC 12 SP 2 RC
       /* C++11 features in VC14 = VC2015 */
#      define ATOMUI_COMPILER_DEFAULT_MEMBERS
#      define ATOMUI_COMPILER_ALIGNAS
#      define ATOMUI_COMPILER_ALIGNOF
#      define ATOMUI_COMPILER_INHERITING_CONSTRUCTORS
#      define ATOMUI_COMPILER_NOEXCEPT
#      define ATOMUI_COMPILER_RANGE_FOR
#      define ATOMUI_COMPILER_REF_QUALIFIERS
#      define ATOMUI_COMPILER_THREAD_LOCAL
#      define ATOMUI_COMPILER_UDL
#      define ATOMUI_COMPILER_UNICODE_STRINGS
#      define ATOMUI_COMPILER_UNRESTRICTED_UNIONS
#    if _MSC_FULL_VER >= 190023419
#      define ATOMUI_COMPILER_ATTRIBUTES
// Almost working, see https://connect.microsoft.com/VisualStudio/feedback/details/2011648
//#      define ATOMUI_COMPILER_CONSTEXPR
#      define ATOMUI_COMPILER_UNIFORM_INIT
#    endif
#    if _MSC_VER >= 1910
#      define ATOMUI_COMPILER_CONSTEXPR
#    endif
// MSVC versions before 19.36 have a bug in C++20 comparison implementation.
// This leads to ambiguities when resolving comparison operator overloads in
// certain scenarios (the buggy MSVC versions were checked using our CI and
// compiler explorer).
#    if _MSC_VER < 1936
#      define ATOMUI_COMPILER_LACKS_THREE_WAY_COMPARE_SYMMETRY
#    endif
#    define ATOMUI_COMPILER_SLOW_QSTRNLEN_COMPILATION
#  endif /* __cplusplus */
#endif // defined(ATOMUI_CC_MSVC) && !defined(ATOMUI_CC_CLANG)

#ifdef ATOMUI_COMPILER_UNICODE_STRINGS
#  define ATOMUI_STDLIB_UNICODE_STRINGS
#elif defined(__cplusplus)
#  error "AtomUI requires Unicode string support in both the compiler and the standard library"
#endif

#ifdef __cplusplus
# include <utility>
# if defined(ATOMUI_OS_QNX)
// By default, QNX 7.0 uses libc++ (from LLVM) and
// QNX 6.X uses Dinkumware's libcpp. In all versions,
// it is also possible to use GNU libstdc++.

// For Dinkumware, some features must be disabled
// (mostly because of library problems).
// Dinkumware is assumed when __GLIBCXX__ (GNU libstdc++)
// and _LIBCPP_VERSION (LLVM libc++) are both absent.
#  if !defined(__GLIBCXX__) && !defined(_LIBCPP_VERSION)

// Older versions of libcpp (QNX 650) do not support C++11 features
// _HAS_* macros are set to 1 by toolchains that actually include
// Dinkum C++11 libcpp.

#   if !defined(_HAS_CPP0X) || !_HAS_CPP0X
// Disable C++11 features that depend on library support
#    undef ATOMUI_COMPILER_INITIALIZER_LISTS
#    undef ATOMUI_COMPILER_RVALUE_REFS
#    undef ATOMUI_COMPILER_REF_QUALIFIERS
#    undef ATOMUI_COMPILER_NOEXCEPT
// Disable C++11 library features:
#    undef ATOMUI_STDLIB_UNICODE_STRINGS
#   endif // !_HAS_CPP0X
#   if !defined(_HAS_NULLPTR_T) || !_HAS_NULLPTR_T
#    undef ATOMUI_COMPILER_NULLPTR
#   endif //!_HAS_NULLPTR_T
#   if !defined(_HAS_CONSTEXPR) || !_HAS_CONSTEXPR
// The libcpp is missing constexpr keywords on important functions like std::numeric_limits<>::min()
// Disable constexpr support on QNX even if the compiler supports it
#    undef ATOMUI_COMPILER_CONSTEXPR
#   endif // !_HAS_CONSTEXPR
#  endif // !__GLIBCXX__ && !_LIBCPP_VERSION
# endif // ATOMUI_OS_QNX
# if defined(ATOMUI_CC_CLANG) && defined(ATOMUI_OS_DARWIN)
#  if defined(__GNUC_LIBSTD__) && ((__GNUC_LIBSTD__-0) * 100 + __GNUC_LIBSTD_MINOR__-0 <= 402)
// Apple has not updated libstdc++ since 2007, which means it does not have
// <initializer_list> or std::move. Let's disable these features
#   undef ATOMUI_COMPILER_INITIALIZER_LISTS
#   undef ATOMUI_COMPILER_RVALUE_REFS
#   undef ATOMUI_COMPILER_REF_QUALIFIERS
// Also disable <atomic>, since it's clearly not there
#   undef ATOMUI_COMPILER_ATOMICS
#  endif
#  if defined(__cpp_lib_memory_resource) \
    && ((defined(__MAC_OS_X_VERSION_MIN_REQUIRED)  && __MAC_OS_X_VERSION_MIN_REQUIRED  < 140000) \
     || (defined(__IPHONE_OS_VERSION_MIN_REQUIRED) && __IPHONE_OS_VERSION_MIN_REQUIRED < 170000))
#   undef __cpp_lib_memory_resource // Only supported on macOS 14 and iOS 17
#  endif
# endif // defined(ATOMUI_CC_CLANG) && defined(ATOMUI_OS_DARWIN)
#endif

// Don't break code that is already using ATOMUI_COMPILER_DEFAULT_DELETE_MEMBERS
#if defined(ATOMUI_COMPILER_DEFAULT_MEMBERS) && defined(ATOMUI_COMPILER_DELETE_MEMBERS)
#  define ATOMUI_COMPILER_DEFAULT_DELETE_MEMBERS
#endif

/*
 * Compatibility macros for C++11/14 keywords and expressions.
 * Don't use in new code and port away whenever you have a chance.
 */
#define ATOMUI_ALIGNOF(x)                alignof(x)
#define ATOMUI_DECL_ALIGN(n)             alignas(n)
#define ATOMUI_DECL_NOTHROW              ATOMUI_DECL_NOEXCEPT
#ifdef __cplusplus
# define ATOMUI_CONSTEXPR                constexpr
# define ATOMUI_DECL_CONSTEXPR           constexpr
# define ATOMUI_DECL_EATOMUI_DEFAULT          = default
# define ATOMUI_DECL_EATOMUI_DELETE           = delete
# define ATOMUI_DECL_FINAL               final
# define ATOMUI_DECL_NOEXCEPT            noexcept
# define ATOMUI_DECL_NOEXCEPT_EXPR(x)    noexcept(x)
# define ATOMUI_DECL_OVERRIDE            override
# define ATOMUI_DECL_RELAXED_CONSTEXPR   constexpr
# define ATOMUI_NULLPTR                  nullptr
# define ATOMUI_RELAXED_CONSTEXPR        constexpr
#else
# define ATOMUI_CONSTEXPR                const
# define ATOMUI_DECL_CONSTEXPR
# define ATOMUI_DECL_RELAXED_CONSTEXPR
# define ATOMUI_NULLPTR                  NULL
# define ATOMUI_RELAXED_CONSTEXPR        const
# ifdef ATOMUI_CC_GNU
#  define ATOMUI_DECL_NOEXCEPT           __attribute__((__nothrow__))
# else
#  define ATOMUI_DECL_NOEXCEPT
# endif
#endif

#if __has_cpp_attribute(nodiscard) && (!defined(ATOMUI_CC_CLANG) || __cplusplus > 201402L) // P0188R1
// Can't use [[nodiscard]] with Clang and C++11/14, see https://bugs.llvm.org/show_bug.cgi?id=33518
#  undef ATOMUI_REQUIRED_RESULT
#  define ATOMUI_REQUIRED_RESULT [[nodiscard]]
#endif

#if __has_cpp_attribute(nodiscard) >= 201907L /* used for both P1771 and P1301... */
// [[nodiscard]] constructor (P1771)
#  ifndef ATOMUI_NODISCARD_CTOR
#    define ATOMUI_NODISCARD_CTOR [[nodiscard]]
#  endif
// [[nodiscard("reason")]] (P1301)
#  ifndef ATOMUI_NODISCARD_X
#    define ATOMUI_NODISCARD_X(message) [[nodiscard(message)]]
#  endif
#  ifndef ATOMUI_NODISCARD_CTOR_X
#    define ATOMUI_NODISCARD_CTOR_X(message) [[nodiscard(message)]]
#  endif
#endif

#if __has_cpp_attribute(maybe_unused)
#  undef ATOMUI_DECL_UNUSED
#  define ATOMUI_DECL_UNUSED [[maybe_unused]]
#endif

#if __has_cpp_attribute(noreturn)
#  undef ATOMUI_NORETURN
#  define ATOMUI_NORETURN [[noreturn]]
#endif

#if __has_cpp_attribute(deprecated)
#  ifdef ATOMUI_DECL_DEPRECATED
#    undef ATOMUI_DECL_DEPRECATED
#  endif
#  ifdef ATOMUI_DECL_DEPRECATED_X
#    undef ATOMUI_DECL_DEPRECATED_X
#  endif
#  define ATOMUI_DECL_DEPRECATED [[deprecated]]
#  define ATOMUI_DECL_DEPRECATED_X(x) [[deprecated(x)]]
#endif

#define ATOMUI_DECL_ENUMERATOR_DEPRECATED ATOMUI_DECL_DEPRECATED
#define ATOMUI_DECL_ENUMERATOR_DEPRECATED_X(x) ATOMUI_DECL_DEPRECATED_X(x)

#ifndef ATOMUI_DECL_CONSTEXPR_DTOR
#  if __cpp_constexpr >= 201907L
#    define ATOMUI_DECL_CONSTEXPR_DTOR constexpr
#  else
#    define ATOMUI_DECL_CONSTEXPR_DTOR inline
#  endif
#endif

#ifndef ATOMUI_CONSTEXPR_DTOR
#  if __cpp_constexpr >= 201907L
#    define ATOMUI_CONSTEXPR_DTOR constexpr
#  else
#    define ATOMUI_CONSTEXPR_DTOR const
#  endif
#endif

#ifndef ATOMUI_DECL_EATOMUI_DELETE_X
// Clang advertises the feature-testing macro but issues a warning
// if one isn't also using C++26,
// https://github.com/llvm/llvm-project/issues/109311
#  if defined(__cpp_deleted_function) && __cpp_deleted_function >= 202403L \
    && (!defined(ATOMUI_CC_CLANG_ONLY) || __cplusplus > 202302L) // C++26
#    define ATOMUI_DECL_EATOMUI_DELETE_X(reason) = delete(reason)
#  else
#    define ATOMUI_DECL_EATOMUI_DELETE_X(reason) = delete
#  endif
#endif

#ifndef ATOMUI_LIKELY_BRANCH
#  if __has_cpp_attribute(likely)
#    define ATOMUI_LIKELY_BRANCH [[likely]]
#    define ATOMUI_UNLIKELY_BRANCH [[unlikely]]
#  else
#    define ATOMUI_LIKELY_BRANCH
#    define ATOMUI_UNLIKELY_BRANCH
#  endif
#endif

/*
 * Fallback macros to certain compiler features
 */

#ifndef ATOMUI_NORETURN
# define ATOMUI_NORETURN
#endif
#ifndef ATOMUI_LIKELY
#  define ATOMUI_LIKELY(x) (x)
#endif
#ifndef ATOMUI_UNLIKELY
#  define ATOMUI_UNLIKELY(x) (x)
#endif
#ifndef ATOMUI_ASSUME_IMPL
#  define ATOMUI_ASSUME_IMPL(expr) atomui_noop()
#endif
#ifndef ATOMUI_UNREACHABLE_IMPL
#  define ATOMUI_UNREACHABLE_IMPL() atomui_noop()
#endif
#ifndef ATOMUI_ALLOC_SIZE
#  define ATOMUI_ALLOC_SIZE(x)
#endif
#ifndef ATOMUI_REQUIRED_RESULT
#  define ATOMUI_REQUIRED_RESULT
#endif
#ifndef ATOMUI_NODISCARD_X
#  define ATOMUI_NODISCARD_X(message) ATOMUI_REQUIRED_RESULT
#endif
#ifndef ATOMUI_NODISCARD_CTOR
#  define ATOMUI_NODISCARD_CTOR
#endif
#ifndef ATOMUI_NODISCARD_CTOR_X
#  define ATOMUI_NODISCARD_CTOR_X(message) ATOMUI_NODISCARD_CTOR
#endif
#ifndef ATOMUI_DECL_DEPRECATED
#  define ATOMUI_DECL_DEPRECATED
#endif
#ifndef ATOMUI_DECL_VARIABLE_DEPRECATED
#  define ATOMUI_DECL_VARIABLE_DEPRECATED ATOMUI_DECL_DEPRECATED
#endif
#ifndef ATOMUI_DECL_DEPRECATED_X
#  define ATOMUI_DECL_DEPRECATED_X(text) ATOMUI_DECL_DEPRECATED
#endif
#ifndef ATOMUI_DECL_EXPORT
#  define ATOMUI_DECL_EXPORT
#endif
#ifndef ATOMUI_DECL_EXPORT_OVERRIDABLE
#  define ATOMUI_DECL_EXPORT_OVERRIDABLE ATOMUI_DECL_EXPORT
#endif
#ifndef ATOMUI_DECL_IMPORT
#  define ATOMUI_DECL_IMPORT
#endif
#ifndef ATOMUI_DECL_HIDDEN
#  define ATOMUI_DECL_HIDDEN
#endif
#ifndef ATOMUI_DECL_UNUSED
#  define ATOMUI_DECL_UNUSED
#endif
#ifndef ATOMUI_DECL_UNUSED_MEMBER
#  define ATOMUI_DECL_UNUSED_MEMBER
#endif
#ifndef ATOMUI_FUNC_INFO
#  if defined(ATOMUI_OS_SOLARIS) || defined(ATOMUI_CC_XLC)
#    define ATOMUI_FUNC_INFO __FILE__ "(line number unavailable)"
#  else
#    define ATOMUI_FUNC_INFO __FILE__ ":" ATOMUI_STRINGIFY(__LINE__)
#  endif
#endif
#ifndef ATOMUI_DECL_CF_RETURNS_RETAINED
#  define ATOMUI_DECL_CF_RETURNS_RETAINED
#endif
#ifndef ATOMUI_DECL_NS_RETURNS_AUTORELEASED
#  define ATOMUI_DECL_NS_RETURNS_AUTORELEASED
#endif
#ifndef ATOMUI_DECL_PURE_FUNCTION
#  define ATOMUI_DECL_PURE_FUNCTION
#endif
#ifndef ATOMUI_DECL_CONST_FUNCTION
#  define ATOMUI_DECL_CONST_FUNCTION ATOMUI_DECL_PURE_FUNCTION
#endif
#ifndef ATOMUI_DECL_COLD_FUNCTION
#  define ATOMUI_DECL_COLD_FUNCTION
#endif

/*
 * Warning/diagnostic handling
 */

#define ATOMUI_DO_PRAGMA(text)                      _Pragma(#text)
#if defined(ATOMUI_CC_MSVC) && !defined(ATOMUI_CC_CLANG)
#  undef ATOMUI_DO_PRAGMA                           /* not needed */
#  define ATOMUI_WARNING_PUSH                       __pragma(warning(push))
#  define ATOMUI_WARNING_POP                        __pragma(warning(pop))
#  define ATOMUI_WARNING_DISABLE_MSVC(number)       __pragma(warning(disable: number))
#  define ATOMUI_WARNING_DISABLE_INTEL(number)
#  define ATOMUI_WARNING_DISABLE_CLANG(text)
#  define ATOMUI_WARNING_DISABLE_GCC(text)
#  define ATOMUI_WARNING_DISABLE_DEPRECATED         ATOMUI_WARNING_DISABLE_MSVC(4996)
#  define ATOMUI_WARNING_DISABLE_FLOAT_COMPARE
#  define ATOMUI_WARNING_DISABLE_INVALID_OFFSETOF
#elif defined(ATOMUI_CC_CLANG)
#  define ATOMUI_WARNING_PUSH                       ATOMUI_DO_PRAGMA(clang diagnostic push)
#  define ATOMUI_WARNING_POP                        ATOMUI_DO_PRAGMA(clang diagnostic pop)
#  define ATOMUI_WARNING_DISABLE_CLANG(text)        ATOMUI_DO_PRAGMA(clang diagnostic ignored text)
#  define ATOMUI_WARNING_DISABLE_GCC(text)
#  define ATOMUI_WARNING_DISABLE_INTEL(number)
#  define ATOMUI_WARNING_DISABLE_MSVC(number)
#  define ATOMUI_WARNING_DISABLE_DEPRECATED         ATOMUI_WARNING_DISABLE_CLANG("-Wdeprecated-declarations")
#  define ATOMUI_WARNING_DISABLE_FLOAT_COMPARE      ATOMUI_WARNING_DISABLE_CLANG("-Wfloat-equal")
#  define ATOMUI_WARNING_DISABLE_INVALID_OFFSETOF   ATOMUI_WARNING_DISABLE_CLANG("-Winvalid-offsetof")
#elif defined(ATOMUI_CC_GNU) && (__GNUC__ * 100 + __GNUC_MINOR__ >= 406)
#  define ATOMUI_WARNING_PUSH                       ATOMUI_DO_PRAGMA(GCC diagnostic push)
#  define ATOMUI_WARNING_POP                        ATOMUI_DO_PRAGMA(GCC diagnostic pop)
#  define ATOMUI_WARNING_DISABLE_GCC(text)          ATOMUI_DO_PRAGMA(GCC diagnostic ignored text)
#  define ATOMUI_WARNING_DISABLE_CLANG(text)
#  define ATOMUI_WARNING_DISABLE_INTEL(number)
#  define ATOMUI_WARNING_DISABLE_MSVC(number)
#  define ATOMUI_WARNING_DISABLE_DEPRECATED         ATOMUI_WARNING_DISABLE_GCC("-Wdeprecated-declarations")
#  define ATOMUI_WARNING_DISABLE_FLOAT_COMPARE      ATOMUI_WARNING_DISABLE_GCC("-Wfloat-equal")
#  define ATOMUI_WARNING_DISABLE_INVALID_OFFSETOF   ATOMUI_WARNING_DISABLE_GCC("-Winvalid-offsetof")
#else       // All other compilers, GCC < 4.6 and MSVC < 2008
#  define ATOMUI_WARNING_DISABLE_GCC(text)
#  define ATOMUI_WARNING_PUSH
#  define ATOMUI_WARNING_POP
#  define ATOMUI_WARNING_DISABLE_INTEL(number)
#  define ATOMUI_WARNING_DISABLE_MSVC(number)
#  define ATOMUI_WARNING_DISABLE_CLANG(text)
#  define ATOMUI_WARNING_DISABLE_GCC(text)
#  define ATOMUI_WARNING_DISABLE_DEPRECATED
#  define ATOMUI_WARNING_DISABLE_FLOAT_COMPARE
#  define ATOMUI_WARNING_DISABLE_INVALID_OFFSETOF
#endif

#ifndef ATOMUI_IGNORE_DEPRECATIONS
#define ATOMUI_IGNORE_DEPRECATIONS(statement) \
    ATOMUI_WARNING_PUSH \
    ATOMUI_WARNING_DISABLE_DEPRECATED \
    statement \
    ATOMUI_WARNING_POP
#endif

// The body must be a statement:
#define ATOMUI_CAST_IGNORE_ALIGN(body) ATOMUI_WARNING_PUSH ATOMUI_WARNING_DISABLE_GCC("-Wcast-align") body ATOMUI_WARNING_POP

// This macro can be used to calculate member offsets for types with a non standard layout.
// It uses the fact that offsetof() is allowed to support those types since C++17 as an optional
// feature. All our compilers do support this, but some issue a warning, so we wrap the offsetof()
// call in a macro that disables the compiler warning.
#define ATOMUI_OFFSETOF(Class, member) \
    []() -> size_t { \
        ATOMUI_WARNING_PUSH ATOMUI_WARNING_DISABLE_INVALID_OFFSETOF \
        return offsetof(Class, member); \
        ATOMUI_WARNING_POP \
    }()

/*
   Proper for-scoping in MIPSpro CC
*/
#ifndef ATOMUI_NO_KEYWORDS
#  if defined(ATOMUI_CC_MIPS) || (defined(ATOMUI_CC_HPACC) && defined(__ia64))
#    define for if (0) {} else for
#  endif
#endif

#ifdef ATOMUI_COMPILER_RVALUE_REFS
#define qMove(x) std::move(x)
#else
#define qMove(x) (x)
#endif

#if defined(__cplusplus)
#if __has_cpp_attribute(clang::fallthrough)
#    define ATOMUI_FALLTHROUGH() [[clang::fallthrough]]
#elif __has_cpp_attribute(gnu::fallthrough)
#    define ATOMUI_FALLTHROUGH() [[gnu::fallthrough]]
#elif __has_cpp_attribute(fallthrough)
#  define ATOMUI_FALLTHROUGH() [[fallthrough]]
#endif
#endif
#ifndef ATOMUI_FALLTHROUGH
#  ifdef ATOMUI_CC_GNU
#    define ATOMUI_FALLTHROUGH() __attribute__((fallthrough))
#  else
#    define ATOMUI_FALLTHROUGH() (void)0
#  endif
#endif

#if defined(__has_attribute) && __has_attribute(uninitialized)
#  define ATOMUI_DECL_UNINITIALIZED __attribute__((uninitialized))
#else
#  define ATOMUI_DECL_UNINITIALIZED
#endif


/*
    Sanitize compiler feature availability
*/
#if !defined(ATOMUI_PROCESSOR_X86)
#  undef ATOMUI_COMPILER_SUPPORTS_SSE2
#  undef ATOMUI_COMPILER_SUPPORTS_SSE3
#  undef ATOMUI_COMPILER_SUPPORTS_SSSE3
#  undef ATOMUI_COMPILER_SUPPORTS_SSE4_1
#  undef ATOMUI_COMPILER_SUPPORTS_SSE4_2
#  undef ATOMUI_COMPILER_SUPPORTS_AVX
#  undef ATOMUI_COMPILER_SUPPORTS_AVX2
#  undef ATOMUI_COMPILER_SUPPORTS_F16C
#endif
#if !defined(ATOMUI_PROCESSOR_ARM)
#  undef ATOMUI_COMPILER_SUPPORTS_NEON
#endif
#if !defined(ATOMUI_PROCESSOR_MIPS)
#  undef ATOMUI_COMPILER_SUPPORTS_MIPS_DSP
#  undef ATOMUI_COMPILER_SUPPORTS_MIPS_DSPR2
#endif

// Compiler version check
#if defined(__cplusplus) && (__cplusplus < 201703L)
#  ifdef ATOMUI_CC_MSVC
#    error "AtomUI requires a C++17 compiler, and a suitable value for __cplusplus. On MSVC, you must pass the /Zc:__cplusplus option to the compiler."
#  else
#    error "AtomUI requires a C++17 compiler"
#  endif
#endif // __cplusplus

#if defined(__cplusplus) && defined(ATOMUI_CC_MSVC) && !defined(ATOMUI_CC_CLANG)
#  if ATOMUI_CC_MSVC < 1927
     // Check below only works with 16.7 or newer
#    error "AtomUI requires at least Visual Studio 2019 version 16.7 (VC++ version 14.27). Please upgrade."
#  endif

// On MSVC we require /permissive- set by user code. Check that we are
// under its rules -- for instance, check that std::nullptr_t->bool is
// not an implicit conversion, as per
// https://docs.microsoft.com/en-us/cpp/overview/cpp-conformance-improvements?view=msvc-160#nullptr_t-is-only-convertible-to-bool-as-a-direct-initialization
static_assert(!std::is_convertible_v<std::nullptr_t, bool>,
              "On MSVC you must pass the /permissive- option to the compiler.");
#endif

#ifdef ATOMUI_PROCESSOR_X86_32
#  if defined(ATOMUI_CC_GNU)
#    define ATOMUI_FASTCALL __attribute__((regparm(3)))
#  elif defined(ATOMUI_CC_MSVC)
#    define ATOMUI_FASTCALL __fastcall
#  else
#    define ATOMUI_FASTCALL
#  endif
#else
#  define ATOMUI_FASTCALL
#endif

// enable gcc warnings for printf-style functions
#if defined(ATOMUI_CC_GNU) && !defined(__INSURE__)
#  if defined(ATOMUI_CC_MINGW) && !defined(ATOMUI_CC_CLANG)
#    define ATOMUI_ATTRIBUTE_FORMAT_PRINTF(A, B) \
         __attribute__((format(gnu_printf, (A), (B))))
#  else
#    define ATOMUI_ATTRIBUTE_FORMAT_PRINTF(A, B) \
         __attribute__((format(printf, (A), (B))))
#  endif
#else
#  define ATOMUI_ATTRIBUTE_FORMAT_PRINTF(A, B)
#endif

#ifdef ATOMUI_CC_MSVC
#  define ATOMUI_NEVER_INLINE __declspec(noinline)
#  define ATOMUI_ALWAYS_INLINE __forceinline
#elif defined(ATOMUI_CC_GNU)
#  define ATOMUI_NEVER_INLINE __attribute__((noinline))
#  define ATOMUI_ALWAYS_INLINE inline __attribute__((always_inline))
#else
#  define ATOMUI_NEVER_INLINE
#  define ATOMUI_ALWAYS_INLINE inline
#endif

//defines the type for the WNDPROC on windows
//the alignment needs to be forced for sse2 to not crash with mingw
#if defined(ATOMUI_OS_WIN)
#  if defined(ATOMUI_CC_MINGW) && defined(ATOMUI_PROCESSOR_X86_32)
#    define ATOMUI_ENSURE_STACK_ALIGNED_FOR_SSE __attribute__ ((force_align_arg_pointer))
#  else
#    define ATOMUI_ENSURE_STACK_ALIGNED_FOR_SSE
#  endif
#  define ATOMUI_WIN_CALLBACK CALLBACK ATOMUI_ENSURE_STACK_ALIGNED_FOR_SSE
#endif

#ifdef __cpp_conditional_explicit
#define ATOMUI_IMPLICIT explicit(false)
#else
#define ATOMUI_IMPLICIT
#endif

#if defined(__cplusplus)

#ifdef __cpp_constinit
# if defined(ATOMUI_CC_MSVC) && !defined(ATOMUI_CC_CLANG)
   // https://developercommunity.visualstudio.com/t/C:-constinit-for-an-optional-fails-if-/1406069
#  define ATOMUI_CONSTINIT
# else
#  define ATOMUI_CONSTINIT constinit
# endif
#elif defined(__has_cpp_attribute) && __has_cpp_attribute(clang::require_constant_initialization)
# define ATOMUI_CONSTINIT [[clang::require_constant_initialization]]
#elif defined(ATOMUI_CC_GNU_ONLY) && ATOMUI_CC_GNU >= 1000
# define ATOMUI_CONSTINIT __constinit
#else
# define ATOMUI_CONSTINIT
#endif

#ifndef ATOMUI_OUTOFLINE_TEMPLATE
#  define ATOMUI_OUTOFLINE_TEMPLATE
#endif
#ifndef ATOMUI_INLINE_TEMPLATE
#  define ATOMUI_INLINE_TEMPLATE inline
#endif

/*
   Avoid some particularly useless warnings from some stupid compilers.
   To get ALL C++ compiler warnings, define ATOMUI_CC_WARNINGS or comment out
   the line "#define ATOMUI_NO_WARNINGS". See also ATOMUIBUG-26877.
*/
#if !defined(ATOMUI_CC_WARNINGS)
#  define ATOMUI_NO_WARNINGS
#endif
#if defined(ATOMUI_NO_WARNINGS)
#  if defined(ATOMUI_CC_MSVC)
ATOMUI_WARNING_DISABLE_MSVC(4251) /* class 'type' needs to have dll-interface to be used by clients of class 'type2' */
ATOMUI_WARNING_DISABLE_MSVC(4244) /* conversion from 'type1' to 'type2', possible loss of data */
ATOMUI_WARNING_DISABLE_MSVC(4275) /* non - DLL-interface classkey 'identifier' used as base for DLL-interface classkey 'identifier' */
ATOMUI_WARNING_DISABLE_MSVC(4514) /* unreferenced inline function has been removed */
ATOMUI_WARNING_DISABLE_MSVC(4800) /* 'type' : forcing value to bool 'true' or 'false' (performance warning) */
ATOMUI_WARNING_DISABLE_MSVC(4097) /* typedef-name 'identifier1' used as synonym for class-name 'identifier2' */
ATOMUI_WARNING_DISABLE_MSVC(4706) /* assignment within conditional expression */
ATOMUI_WARNING_DISABLE_MSVC(4355) /* 'this' : used in base member initializer list */
ATOMUI_WARNING_DISABLE_MSVC(4710) /* function not inlined */
ATOMUI_WARNING_DISABLE_MSVC(4530) /* C++ exception handler used, but unwind semantics are not enabled. Specify /EHsc */
#  elif defined(ATOMUI_CC_BOR)
#    pragma option -w-inl
#    pragma option -w-aus
#    pragma warn -inl
#    pragma warn -pia
#    pragma warn -ccc
#    pragma warn -rch
#    pragma warn -sig
#  endif
#endif

#if !defined(ATOMUI_NO_EXCEPTIONS)
#  if !defined(ATOMUI_MOC_RUN)
#    if defined(ATOMUI_CC_GNU) && !defined(__cpp_exceptions)
#      define ATOMUI_NO_EXCEPTIONS
#    endif
#  elif defined(ATOMUI_BOOTSTRAPPED)
#    define ATOMUI_NO_EXCEPTIONS
#  endif
#endif

// libstdc++ shipped with gcc < 11 does not have a fix for defect LWG 3346
#if __cplusplus >= 202002L && (!defined(_GLIBCXX_RELEASE) || _GLIBCXX_RELEASE >= 11)
#  define ATOMUI_COMPILER_HAS_LWG3346
#endif

#if defined(__cplusplus) && __cplusplus >= 202002L // P0846 doesn't have a feature macro :/
# if !defined(ATOMUI_CC_MSVC_ONLY) || ATOMUI_CC_MSVC >= 1939 // claims C++20 support but lacks P0846
                                                   // 1939 is known to work
                                                   // 1936 is known to fail
#  define ATOMUI_COMPILER_HAS_P0846
# endif
#endif

#ifdef ATOMUI_COMPILER_HAS_P0846
#   define ATOMUI_ENABLE_P0846_SEMANTICS_FOR(func)
#else
    class ATOMUI_CLASS_JUST_FOR_P0846_SIMULATION;
#   define ATOMUI_ENABLE_P0846_SEMANTICS_FOR(func) \
        template <typename T> \
        void func (ATOMUI_CLASS_JUST_FOR_P0846_SIMULATION *); \
        /* end */
#endif

#endif // __cplusplus
