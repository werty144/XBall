package com.xballserver.localserver

import com.xballserver.remoteserver.routing.createServerReadyJSONString
import io.ktor.serialization.kotlinx.json.*
import io.ktor.server.application.*
import io.ktor.server.plugins.contentnegotiation.*

fun main(args: Array<String>): Unit = io.ktor.server.netty.EngineMain.main(args)


@Suppress("unused") // Referenced in application.conf
@kotlin.jvm.JvmOverloads
fun Application.module(testing: Boolean = false) {
    install(ContentNegotiation) {
        json()
    }

    val printer = Printer()
    val gameManager = GameManager(printer)

    configureRouting(gameManager)
}

fun Application.events() {
    environment.monitor.subscribe(ApplicationStarted, ::onStarted)
}

fun onStarted(application: Application) {
    println(createServerReadyJSONString())
}
