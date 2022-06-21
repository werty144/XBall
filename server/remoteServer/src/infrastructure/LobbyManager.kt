package com.xballserver.remoteserver.infrastructure

import com.example.game.GameProperties
import kotlinx.coroutines.GlobalScope
import kotlinx.coroutines.launch
import kotlinx.serialization.Transient
import java.sql.Timestamp
import java.util.*
import kotlin.collections.LinkedHashSet

typealias LobbyID = ULong
typealias UserId = ULong

class LobbyManager(val gamesManager: GamesManager) {
    val lobbies: MutableSet<Lobby> = Collections.synchronizedSet(LinkedHashSet())
    val lobbyOutDateTime = 20_000L

    fun lobbyReady(lobbyID: LobbyID, userId: UserId, nMembers: Int, gameProperties: GameProperties) {
        if (!lobbies.any { it.id == lobbyID }) {
            val newLobby = Lobby(lobbyID, nMembers, gameProperties)
            lobbies.add(newLobby)
        }

        val lobby = lobbies.find { it.id == lobbyID }!!
        lobby.members.add(userId)
        if (lobby.members.size == nMembers) {
            GlobalScope.launch { gamesManager.startGameFromLobby(lobby) }
            lobbies.remove(lobby)
        }
    }

    fun clean() {
        val currentTimeStamp = Timestamp(System.currentTimeMillis())
        lobbies.removeIf { currentTimeStamp.time - it.timeStamp.time > lobbyOutDateTime }
    }
}

class Lobby(
    val id: LobbyID,
    val nMembers: Int,
    val gameProperties: GameProperties,
    val members: MutableSet<UserId> = mutableSetOf(),
    val timeStamp: Timestamp = Timestamp(System.currentTimeMillis())
    )
{
    override fun toString(): String = "Lobby(users: ${members})"
}