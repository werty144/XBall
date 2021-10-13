#include <iostream>
#include <cpr/cpr.h>

int main(int argc, char** argv) {
    auto response = cpr::Get(cpr::Url{"https://example.com"});
    std::cout << response.text << std::endl;
}