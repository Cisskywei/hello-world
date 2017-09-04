var UniversalMediaPlayer = {
	states: {
        Empty: 0,
        Opening: 1,
        Buffering: 2,
        Prepared: 3,
        Playing: 4,
        Paused: 5,
        Stopped: 6,
        EndReached: 7,
        EncounteredError: 8,
        TimeChanged: 9,
        PositionChanged: 10,
        SnapshotTaken: 11,
    },
    players: [],
    UMPNativeInit__deps: ["states", "players"],
    UMPNativeInit: function () {
        var player = document.createElement("video");
		player.crossOrigin = "anonymous";
        //player.setAttribute('crossOrigin', 'anonymous');
		
		var playerState = {
			state: _states.Empty,
			valueFloat: -1,
			valueLong: -1,
			valueString: undefined,
		};
		
		var playerData = {
            player: player,
			ready: false,
			playerState: {state: _states.Empty, value: undefined},
            playerStates: []
        };
		
		_players.push(playerData);

		player.oncanplay = function () {
			playerData.ready = true;
		};
		
        player.onplaying = function () {
			playerData.playerStates.push({state: _states.Playing, valueFloat: -1, valueLong: -1, valueString: undefined});
        };

        player.onwaiting = function () {
			playerData.playerStates.push({state: _states.Buffering, valueFloat: 0, valueLong: -1, valueString: undefined});
        };

        player.onpause = function () {
			playerData.playerStates.push({state: _states.Paused, valueFloat: -1, valueLong: -1, valueString: undefined});
        };

        player.onended = function () {
			playerData.playerStates.push({state: _states.EndReached, valueFloat: -1, valueLong: -1, valueString: undefined});
        };

        player.ontimeupdate = function() {
			playerData.playerStates.push({state: _states.PositionChanged, valueFloat: player.currentTime / player.duration, valueLong: -1, valueString: undefined});
			playerData.playerStates.push({state: _states.TimeChanged, valueFloat: -1, valueLong: player.currentTime * 1000, valueString: undefined});
        };
		 
        player.onerror = function (message) {
			playerData.playerStates.push({state: _states.EncounteredError, valueFloat: -1, valueLong: -1, valueString: undefined});
            var errormessage = "Undefined error";

            switch (this.error.code) {
                case 1:
                    err = "Fetching process aborted by user";
                    break;
                case 2:
                    err = "Error occurred when downloading";
                    break;
                case 3:
                    err = "Error occurred when decoding";
                    break;
                case 4:
                    err = "Audio/Video not supported";
                    break;
            }

            console.log(err + " (errorcode=" + this.error.code + ")");
        };

		return _players.length - 1;
    },
	UMPNativeInitPlayer__deps: ["players"],
	UMPNativeInitPlayer: function (index, options) {
		
	},
    UMPNativeUpdateTexture__deps: ["players"],
    UMPNativeUpdateTexture: function (index, texture) {
        GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[texture]);
        GLctx.pixelStorei(GLctx.UNPACK_FLIP_Y_WEBGL, true);
        GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_WRAP_S, GLctx.CLAMP_TO_EDGE);
        GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_WRAP_T, GLctx.CLAMP_TO_EDGE);
        GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_MIN_FILTER, GLctx.LINEAR);
        GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_MAG_FILTER, GLctx.LINEAR);
        GLctx.texImage2D(GLctx.TEXTURE_2D, 0, GLctx.RGB, GLctx.RGB, GLctx.UNSIGNED_BYTE, _players[index].player);
		GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[0]);
		GLctx.pixelStorei(GLctx.UNPACK_FLIP_Y_WEBGL, false);
    },
	UMPSetDataSource__deps: ["players"],
    UMPSetDataSource: function (index, path) {
		_players[index].player.src = Pointer_stringify(path);
		_players[index].player.load();
    },
	UMPPlay__deps: ["players"],
    UMPPlay: function (index) {
        _players[index].player.play();
		return true;
    },
	UMPPause__deps: ["players"],
    UMPPause: function (index) {
        _players[index].player.pause();
    },
	UMPStop__deps: ["players"],
    UMPStop: function (index) {
        var player = _players[index].player;
        player.pause();
        player.currentTime = 0;
		_players[index].ready = false;
    },
	UMPRelease__deps: ["players"],
    UMPRelease: function (index) {
        var player = _players[index].player;
        player.src = "";
        player.load();
		player.parentNode.removeChild(vid);
		
		_players[index].player = null;
    },
	UMPIsPlaying__deps: ["players"],
    UMPIsPlaying: function (index) {
        var player = _players[index].player;
        return !(player.paused || player.ended || player.seeking || player.readyState < player.HAVE_FUTURE_DATA);
    },
	UMPIsReady__deps: ["players"],
    UMPIsReady: function (index) {
		return _players[index].ready;
    },
	UMPGetLength__deps: ["players"],
    UMPGetLength: function (index) {
        return _players[index].player.duration * 1000;
    },
	UMPGetTime__deps: ["players"],
    UMPGetTime: function (index) {
        return _players[index].player.currentTime * 1000;
    },
    UMPSetTime__deps: ["players"],
    UMPSetTime: function (index, time) {
		_players[index].player.currentTime = time * 1000;
    },
	UMPGetPosition__deps: ["players"],
    UMPGetPosition: function (index) {
	    var player = _players[index].player;
        return player.currentTime / player.duration;
    },
    UMPSetPosition__deps: ["players"],
    UMPSetPosition: function (index, position) {
		var player = _players[index].player;
		player.currentTime = position * player.duration;
    },
    UMPGetRate__deps: ["players"],
    UMPGetRate: function (index) {
        return _players[index].player.playbackRate;
    },
    UMPSetRate__deps: ["players"],
    UMPSetRate: function (index, rate) {
        _players[index].player.playbackRate = rate;
    },
	UMPGetVolume__deps: ["players"],
    UMPGetVolume: function (index) {
        return _players[index].player.volume * 100;
    },
    UMPSetVolume__deps: ["players"],
    UMPSetVolume: function (index, volume) {
        _players[index].player.volume = volume / 100;
    },
	UMPGetMute__deps: ["players"],
    UMPGetMute: function (index) {
        return _players[index].player.muted;
    },
	UMPSetMute__deps: ["players"],
    UMPSetMute: function (index, mute) {
        _players[index].player.muted = mute;
    },
	UMPVideoWidth__deps: ["players"],
    UMPVideoWidth: function (index) {
    	return _players[index].player.videoWidth;
    },
    UMPVideoHeight__deps: ["players"],
    UMPVideoHeight: function (index) {
        return _players[index].player.videoHeight;
    },
	UMPVideoFrameCount__deps: ["players"],
    UMPVideoFrameCount: function (index) {
        var player = _players[index].player;

        var frameCount = 0;
        if (player.webkitDecodedFrameCount) {
        	frameCount = player.webkitDecodedFrameCount;
        }

        if (player.mozDecodedFrames) {
        	frameCount = player.mozDecodedFrames;
        }

        return frameCount;
    },
	UMPGetState__deps: ["states", "players"],
    UMPGetState: function (index) {
		var playerStates = _players[index].playerStates;
		if (playerStates.length > 0) {
			_players[index].playerState	= playerStates.shift();
			return _players[index].playerState.state;
		}
		
		return _states.Empty;
    },
	UMPGetStateFloatValue__deps: ["states", "players"],
    UMPGetStateFloatValue: function (index) {
		if (_players[index].playerState != undefined) {
			return _players[index].playerState.valueFloat;
		}
		
		return -1;
    },
	UMPGetStateLongValue__deps: ["states", "players"],
    UMPGetStateLongValue: function (index) {
		if (_players[index].playerState != undefined) {
			return _players[index].playerState.valueLong;
		}
		
		return -1;
    },
	UMPGetStateStringValue__deps: ["states", "players"],
    UMPGetStateStringValue: function (index) {
		if (_players[index].playerState != undefined) {
			return _players[index].playerState.valueString;
		}
		
		return undefined;
    },
};

autoAddDeps(UniversalMediaPlayer, 'states');
autoAddDeps(UniversalMediaPlayer, 'players');
mergeInto(LibraryManager.library, UniversalMediaPlayer);