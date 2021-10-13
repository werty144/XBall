#include <iostream>
#include <cpr/cpr.h>
#include <nlohmann/json.hpp>

// for convenience
using json = nlohmann::json;

using namespace std;


int Undefined = -1;
struct SessionData {
    int userId = Undefined;
    int gameId = Undefined;
};

int main(int argc, char** argv) {
    string host_url = "http://0.0.0.0:8080";
    auto session_data = SessionData{};

    bool exit = false;
    while (not exit) {
        int command;
        cout << "Enter command number" << endl;
        cin >> command;
        switch (command) {
            case 0:
                exit = true;
                break;
            case 1:
                {
                    auto response = cpr::Get(cpr::Url{host_url + "/getUserId"});
                    session_data.userId = stoi(response.text);
                    cout << "Your user Id is now " + response.text << endl;
                    break;
                }
            case 2:
                {
                    if (session_data.userId == Undefined) {
                        cout << "Get Id first" << endl;
                        break;
                    }
                    int invited_id;
                    cout << "Enter invited player id" << endl;
                    cin >> invited_id;
                    auto response = cpr::Post
                            (
                                cpr::Url{host_url + "/gameInvite"},
                                cpr::Payload
                                (
                                {
                                        {"inviter_id", to_string(session_data.userId)},
                                        {"invited_id", to_string(invited_id)}
                                    }
                                )
                            );
                    cout << response.status_code << endl;
                    break;
                }
            case 3:
                {
                    auto response = cpr::Post
                            (
                                cpr::Url{host_url + "/checkInvites"},
                                cpr::Payload
                                (
                                    {{"user_id", to_string(session_data.userId)}}
                                )
                            );
                    json invites = json::parse(response.text);
                    if (invites.empty()) cout << "No invites";
                    for (auto & invite : invites) {
                        cout << invite << ' ';
                    }
                    cout << endl;
                    break;
                }
            case 4:
                {
                    int invite_id;
                    cout << "Enter invite id to accept" << endl;
                    cin >> invite_id;
                    auto response = cpr::Post
                            (
                                cpr::Url{host_url + "/acceptInvite"},
                                cpr::Payload
                                (
                                {
                                    {"user_id", to_string(session_data.userId)},
                                    {"invite_id", to_string(invite_id)}
                                }
                                )
                            );
                    cout << response.status_code<< endl;
                    break;
                }
            case 5:
                {
                    auto response = cpr::Post
                        (
                            cpr::Url{host_url + "/checkGames"},
                            cpr::Payload
                                (
                                    {{"user_id", to_string(session_data.userId)}}
                                )
                        );
                    json games = json::parse(response.text);
                    if (games.empty()) cout << "No games";
                    for (auto & game : games) {
                        cout << game << ' ';
                    }
                    cout << endl;
                    break;
                }
            case 6:
                {
                    int game_id;
                    cout << "Enter game id" << endl;
                    cin >> game_id;
                    auto response = cpr::Post
                            (
                                cpr::Url{host_url + "/getGameState"},
                                cpr::Payload
                                    (
                                    {
                                            {"game_id", to_string(game_id)}
                                        }
                                    )
                            );
                    cout << response.text << endl;
                    break;
                }
            case 7:
                {
                    int game_id, new_state;
                    cout << "Enter game id and new state" << endl;
                    cin >> game_id >> new_state;
                    auto response = cpr::Post
                        (
                            cpr::Url{host_url + "/setGameState"},
                            cpr::Payload
                                (
                                    {
                                        {"game_id", to_string(game_id)},
                                        {"new_state", to_string(new_state)}
                                    }
                                )
                        );
                    cout << response.status_code << endl;
                    break;
                }
            default:
                cout << "No such command" << endl;
        }
    }
}