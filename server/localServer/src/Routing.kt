package com.xballserver.localserver

import com.example.game.GameProperties
import com.xballserver.remoteserver.infrastructure.Lobby
import com.xballserver.remoteserver.infrastructure.LobbyID
import com.xballserver.remoteserver.routing.APIMove
import io.ktor.http.*
import io.ktor.server.application.*
import io.ktor.server.request.*
import io.ktor.server.response.*
import io.ktor.server.routing.*
import kotlinx.serialization.json.JsonElement


fun Application.configureRouting(gameManager: GameManager)
{
    routing {
        post("/lobbyReady") {
            val lobby = call.receive<SerializableLobby>().toLobby()
            gameManager.startGameFromLobby(lobby)
            call.respond(HttpStatusCode.OK)
        }

        post("/makeMove") {
            val move = call.receive<APIMoveAddressantS>().toAPIMoveAddressant()
            gameManager.makeMove(move.getAPIMove(), move.addressant)
        }

        get("/test") {
            call.respondText("Privet")
            call.respond(HttpStatusCode.OK)
        }
    }
}


// Replacing all ULongs with String since their deserialization is not supported
@kotlinx.serialization.Serializable
class SerializableLobby(
    val id: String,
    val nMembers: Int,
    val gameProperties: GameProperties,
    val members: MutableSet<String> = mutableSetOf()
) {
    fun toLobby() = Lobby(
            id.toULong(),
            nMembers,
            gameProperties,
            members.map { it.toULong() }.toMutableSet()
        )
}

class APIMoveAddressant(
    val addressant: ULong,
    val playerId: Int,
    val action: String,
    val actionData: JsonElement
) {
    fun getAPIMove() = APIMove(playerId, action, actionData)
}

// Replacing all ULong with String since their deserialization is not supported
@kotlinx.serialization.Serializable
class APIMoveAddressantS(
    val addressant: String,
    val playerId: Int,
    val action: String,
    val actionData: JsonElement
) {
    fun toAPIMoveAddressant() = APIMoveAddressant(addressant.toULong(), playerId, action, actionData)
}
