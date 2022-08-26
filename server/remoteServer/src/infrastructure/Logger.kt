package com.xballserver.remoteserver.infrastructure

import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.withContext
import java.io.File
import java.io.OutputStream
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter


class Logger(val lobbyManager: LobbyManager, val gamesManager: GamesManager, val connectionManager: ConnectionManager) {
    suspend fun logPeriodically(period: Long = 3000L, outputStream: OutputStream = System.out) {
        val dtf = DateTimeFormatter.ofPattern("yyyy/MM/dd HH:mm:ss")
        while (true) {
            delay(period)
            val now = LocalDateTime.now()
            withContext(Dispatchers.IO) {
                outputStream.write((
                        "${dtf.format(now)}\n" +
                        "${connectionManager.connections.size} Connections: ${connectionManager.connections}\n" +
                        "Messages sent: ${connectionManager.getAndNullifyMessagesSent()}\n" +
                        "${gamesManager.games.size} Games: ${gamesManager.games}\n" +
                        "${gamesManager.runningGames.size} Running games: ${gamesManager.runningGames}\n" +
                        "${lobbyManager.lobbies.size} Lobbies: ${lobbyManager.lobbies}\n" +
                        "\n"
                        ).toByteArray())
            }
        }
    }
}

fun logToFile(message: String, fileName: String="serverLog.txt")
{
    File(fileName).appendText(message + '\n')
}
