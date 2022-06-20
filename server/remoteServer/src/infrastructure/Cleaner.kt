package com.example.infrastructure

import com.example.routing.Connections
import kotlinx.coroutines.delay
import kotlinx.coroutines.isActive

class Cleaner(val lobbyManager: LobbyManager, val gamesManager: GamesManager, val connections: Connections) {

    private val inviteOutdatedTime = 60000L

    suspend fun cleanPeriodically(period: Long = 3000L) {
        while (true) {
            delay(period)
            lobbyManager.clean()
            gamesManager.clean()
            connections.removeIf { !it.session.isActive }
        }
    }
}