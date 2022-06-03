package com.example.infrastructure

import io.ktor.http.cio.websocket.*
import java.net.URI
import java.net.URLEncoder
import java.net.http.HttpClient
import java.net.http.HttpRequest
import java.net.http.HttpResponse
import kotlinx.serialization.*
import kotlinx.serialization.json.Json
import java.io.File


class AuthenticationManager {
    private val client: HttpClient = HttpClient.newBuilder().build()
    private val passwordsToUsers: MutableMap<String, UserId> = mutableMapOf()
    private val steamworksWebApiKey: String = File("SteamworksWebApiKey").readText(Charsets.UTF_8)
    fun generatePasswordForUser(steamId: UserId): String {
        passwordsToUsers[steamId.toString()] = steamId
        return steamId.toString()
    }

    fun validateSteamTicket(ticket: String): UserId? {
        val params = mapOf("ticket" to ticket, "appid" to "480", "key" to steamworksWebApiKey)
        val urlParams = params.map {(k, v) -> "${(k.utf8())}=${v.utf8()}"}
            .joinToString("&")
        val request = HttpRequest.newBuilder()
            .uri(URI.create("https://partner.steam-api.com/ISteamUserAuth/AuthenticateUserTicket/v1/?${urlParams}"))
            .build();
        val response = client.send(request, HttpResponse.BodyHandlers.ofString());
        val content = Json.decodeFromString<SteamAnswer>(response.body()).response.params

        if (content.result != "OK") return null

        return content.steamid.toLong()
    }

    fun validateFirstMessage(frame: Frame): Boolean {
        if (frame !is Frame.Text) return false
        val password = frame.readText()
        return passwordsToUsers.keys.contains(password)
    }

    fun userId(password: String) = passwordsToUsers[password]
}

fun String.utf8(): String = URLEncoder.encode(this, "UTF-8")

@Serializable
data class UserCredentials(val ticket: String, val publicKey: String)

@Serializable
class SteamAnswer(
    val response: SteamResponse
)
@Serializable
class SteamResponse(
    val params: SteamParams
)
@Serializable
class SteamParams(
    val result: String,
    val steamid: String,
    val ownersteamid: String,
    val vacbanned: Boolean,
    val publisherbanned: Boolean
)
