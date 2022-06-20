package com.example.infrastructure

import com.example.routing.Connections
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.withContext
import java.io.OutputStream

class Logger(val lobbyManager: LobbyManager, val gamesManager: GamesManager, val connections: Connections) {
    suspend fun logPeriodically(period: Long = 3000L, outputStream: OutputStream = System.out) {
        while (true) {
            delay(period)
            withContext(Dispatchers.IO) {
                outputStream.write((
                        "${connections.size} Connections: $connections\n" +
                        "${gamesManager.games.size} Games: ${gamesManager.games}\n" +
                                "${lobbyManager.lobbies.size} Lobbies: ${lobbyManager.lobbies}\n" +
                                "\n"
                        ).toByteArray())
            }
        }
    }
}