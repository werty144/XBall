package com.example.infrastructure

import com.example.routing.Connections
import kotlinx.coroutines.delay
import kotlinx.coroutines.isActive
import java.sql.Timestamp

class Cleaner(val invitesManager: InvitesManager, val gamesManager: GamesManager, val connections: Connections) {

    private val inviteOutdatedTime = 60000L

    suspend fun cleanPeriodically(period: Long = 3000L) {
        while (true) {
            delay(period)
            val currentTimeStamp = Timestamp(System.currentTimeMillis())

            invitesManager.invites.removeIf { currentTimeStamp.time - it.timeStamp.time > inviteOutdatedTime}
            connections.removeIf { !it.session.isActive }
        }
    }
}