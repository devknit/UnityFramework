# Framework

本パッケージは１つの画面をモジュールという単位で制御し、それぞれのモジュールの行き来を行う根幹の仕組みを提供することを目的としています。

行き来を行うためのイベントを発行するために、Unity に元来からある UI と同等のコンポーネントも提供しています。

提供している機能は以下になります

- Module
	- [シーンモジュール](documentation~/scenemodule.md)
	- [モーダルモジュール](documentation~/modalmodule.md)
	- [アンビエントモジュール](documentation~/ambientmodule.md)
- UI
	- ボタン
	- トグル

また本パッケージは以下のパッケージに依存しています
- Addressable
- URP