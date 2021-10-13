cmake_minimum_required(VERSION 3.20)
project(client)

include_directories()

set(CMAKE_CXX_STANDARD 14)

set(OpenGlLinkers -lglut -lGLU -lGL -lGLEW)

add_executable(client main.cpp)

target_link_libraries(client PRIVATE ${OpenGlLinkers} cpr::cpr)

include(FetchContent)
FetchContent_Declare(cpr GIT_REPOSITORY https://github.com/libcpr/cpr.git GIT_TAG f4622efcb59d84071ae11404ae61bd821c1c344b) # the commit hash for 1.6.2
FetchContent_MakeAvailable(cpr)

