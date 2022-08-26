package com.xballserver.remoteserver.infrastructure

import com.xballserver.remoteserver.routing.Connections
import kotlinx.coroutines.delay
import kotlinx.coroutines.isActive

class Cleaner(val lobbyManager: LobbyManager, val gamesManager: GamesManager, val connectionManager: ConnectionManager) {
    suspend fun cleanPeriodically(period: Long = 3000L) {
        while (true) {
            delay(period)
            lobbyManager.clean()
            gamesManager.clean()
            connectionManager.clean()
        }
    }
}