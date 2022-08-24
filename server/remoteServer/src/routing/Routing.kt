package com.xballserver.remoteserver.routing

import com.fasterxml.jackson.databind.SerializationFeature
import com.fasterxml.jackson.dataformat.xml.JacksonXmlModule
import com.fasterxml.jackson.dataformat.xml.XmlMapper
import com.fasterxml.jackson.dataformat.xml.annotation.JacksonXmlProperty
import com.fasterxml.jackson.dataformat.xml.annotation.JacksonXmlRootElement
import com.xballserver.remoteserver.infrastructure.*

import io.ktor.http.*
import io.ktor.server.application.*
import io.ktor.server.request.*
import io.ktor.server.response.*
import io.ktor.server.routing.*
import io.ktor.server.websocket.*
import io.ktor.websocket.*
import kotlinx.coroutines.isActive
import java.math.BigInteger
import java.security.KeyFactory
import java.security.spec.RSAPublicKeySpec
import java.util.*
import java.util.concurrent.atomic.AtomicInteger
import javax.crypto.Cipher


typealias Connections = MutableSet<Connection>

fun Application.configureRouting(gamesManager: GamesManager,
                                 authenticationManager: AuthenticationManager,
                                 connectionManager: ConnectionManager,
                                 gameStartManager: GameStartManager
) {

    val apiHandler = APIHandler(gamesManager, gameStartManager)

    routing {
        post("/auth") {
            val credentials = call.receive<UserCredentials>()
            val steamId = authenticationManager.validateSteamTicket(credentials.ticket) ?: return@post
            val superSecretPassword = authenticationManager.generatePasswordForUser(steamId)

            val publicKey = getPublicKey(credentials)
            val encodedPassword = encodePassword(publicKey, superSecretPassword)

            call.response.status(HttpStatusCode.OK)
            call.respond(encodedPassword)
        }

        webSocket("/") {
            var firstMessageReceived = false
            var thisConnection: Connection? = null
            for (frame in incoming) {
                if (!firstMessageReceived) {
                    firstMessageReceived = true
                    if (!authenticationManager.validateFirstMessage(frame)) {
                        close()
                        return@webSocket
                    } else {
                        val password = (frame as Frame.Text).readText()
                        thisConnection = Connection(this, authenticationManager.userId(password)!!)
                        connectionManager.addConnection(thisConnection)
                    }
                } else {
//                    apiHandler.handle(frame, thisConnection!!.userId)
                }
            }
        }

        get("/stop_games") {
            gamesManager.stopAll()
        }

        get("/test") {
            call.respond("Poshel nahuy")
        }

    }
}

fun getPublicKey(credentials: UserCredentials): RSAKeyValue {
    val xmlMapper = XmlMapper(
        JacksonXmlModule().apply { setDefaultUseWrapper(false) }
    ).apply {
        enable(SerializationFeature.WRAP_ROOT_VALUE)
    }
    val publicKey: RSAKeyValue =
        xmlMapper.readValue(credentials.publicKey, RSAKeyValue::class.java)
    return publicKey
}

fun encodePassword(publicKey: RSAKeyValue, password: String): ByteArray {
    val decoder: Base64.Decoder = Base64.getDecoder()
    val modulusBytes: ByteArray = decoder.decode(publicKey.Modulus)
    val exponentBytes: ByteArray = decoder.decode(publicKey.Exponent)

    val modulus = BigInteger(1, modulusBytes)
    val exponent = BigInteger(1, exponentBytes)

    val rsaPubKey = RSAPublicKeySpec(modulus, exponent)
    val fact = KeyFactory.getInstance("RSA")
    val pubKey = fact.generatePublic(rsaPubKey)

    val cipher = Cipher.getInstance("RSA/ECB/PKCS1Padding")
    cipher.init(Cipher.ENCRYPT_MODE, pubKey)

    val bytes = password.toByteArray()

    return cipher.doFinal(bytes)
}

class Connection(val session: DefaultWebSocketSession, val userId: UserId) {
    companion object {
        var lastId = AtomicInteger(0)
    }

    val id = lastId.getAndIncrement()

    override fun toString(): String = "Connection(userId=$userId, active=${session.isActive})"
}

@JacksonXmlRootElement(localName = "RSAKeyValue")
data class RSAKeyValue(
    @JacksonXmlProperty(localName = "Modulus")
    val Modulus: String,
    @JacksonXmlProperty(localName = "Exponent")
    val Exponent: String)