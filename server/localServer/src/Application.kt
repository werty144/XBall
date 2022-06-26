package com.xballserver.localserver

import com.xballserver.remoteserver.routing.createServerReadyJSONString
import io.ktor.serialization.kotlinx.json.*
import io.ktor.server.application.*
import io.ktor.server.engine.*
import io.ktor.server.netty.*
import io.ktor.server.plugins.contentnegotiation.*
import io.ktor.server.websocket.*
import kotlinx.coroutines.CoroutineName
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.launch
import kotlinx.coroutines.runBlocking
import kotlin.coroutines.CoroutineContext


var server: NettyApplicationEngine? = null

fun main(args: Array<String>) {
    server = embeddedServer(Netty, environment = applicationEngineEnvironment {
        module {
            module()
            events()
        }

        connector {
            port = 0
            host = "127.0.0.1"
        }
    }
    )
    server!!.start(wait = true)
}


@Suppress("unused") // Referenced in application.conf
@kotlin.jvm.JvmOverloads
fun Application.module(testing: Boolean = false) {
    install(WebSockets)
    install(ContentNegotiation) {
        json()
    }

    val gameManager = GameManager()

    configureRouting(gameManager)
}

fun Application.events() {
    environment.monitor.subscribe(ApplicationStarted, ::onStarted)
}

fun onStarted(application: Application) {
    CoroutineScope(CoroutineName("Initial message")).launch {
        val port = server!!.resolvedConnectors().first().port
        println(createServerReadyJSONString(port))
    }
}
