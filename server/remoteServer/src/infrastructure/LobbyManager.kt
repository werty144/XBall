package com.xballserver.remoteserver.infrastructure

import com.xballserver.remoteserver.game.GameProperties
import java.sql.Timestamp
import java.util.*
import kotlin.collections.LinkedHashSet

typealias LobbyID = ULong
typealias UserId = ULong

class LobbyManager(val gamesManager: GamesManager) {
    val lobbies: MutableSet<Lobby> = Collections.synchronizedSet(LinkedHashSet())

    fun createLobby(lobbyID: LobbyID, nMembers: Int, gameProperties: GameProperties) {
        val newLobby = Lobby(lobbyID, nMembers, gameProperties)
        lobbies.add(newLobby)
    }

    fun getLobby(lobbyID: LobbyID): Lobby? {
        return lobbies.find { it.id == lobbyID }
    }

    fun addUserToLobby(lobbyID: LobbyID, userId: UserId) {
        val lobby = lobbies.find { it.id == lobbyID } ?: return
        lobby.members.add(userId)
    }

    fun exists(lobbyID: LobbyID): Boolean {
        return lobbies.any { it.id == lobbyID }
    }

//    suspend fun lobbyReady(lobbyID: LobbyID, userId: UserId, nMembers: Int, gameProperties: GameProperties) {
//        if (!lobbies.any { it.id == lobbyID }) {
//            val newLobby = Lobby(lobbyID, nMembers, gameProperties)
//            lobbies.add(newLobby)
//        }
//
//        val lobby = lobbies.find { it.id == lobbyID }!!
//        lobby.members.add(userId)
//        if (lobby.members.size == nMembers) {
//            coroutineScope {
//                launch { gamesManager.startGameFromLobby(lobby) }
//            }
//            lobbies.remove(lobby)
//        }
//    }

    fun allReady(lobbyID: LobbyID): Boolean {
        val lobby = lobbies.find { it.id == lobbyID } ?: return false
        return lobby.members.size == lobby.maxCapacity
    }

    fun remove(lobbyID: LobbyID) {
        val lobby = lobbies.find { it.id == lobbyID } ?: return
        lobbies.remove(lobby)
    }

    fun clean() {
        lobbies.removeIf { it.isOutDated() }
    }
}

class Lobby(
    val id: LobbyID,
    val maxCapacity: Int,
    val gameProperties: GameProperties,
    val members: MutableSet<UserId> = Collections.synchronizedSet(LinkedHashSet()),
    val timeStamp: Timestamp = Timestamp(System.currentTimeMillis()),
    val outDateTime: Long = 5_000L
    )
{
    fun isOutDated(): Boolean {
        val currentTimeStamp = Timestamp(System.currentTimeMillis())
        return currentTimeStamp.time - timeStamp.time > outDateTime
    }
    override fun toString(): String = "Lobby(users: ${members})"
}