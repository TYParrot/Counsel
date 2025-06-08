# VR 기반 실시간 AI 상담 시스템

## 📌 프로젝트 개요
본 프로젝트는 텍스트 기반 또는 제한적인 음성 인터랙션이 가지는 몰입도 한계를 보완하고자,  
**VR 환경에서 실시간 상담이 가능한 시스템**을 구축하기 위해 기획되었습니다.  
보다 **직관적이고 몰입감 있는 사용자 경험**을 제공하는 것을 목표로 합니다.

---

## 🛠️ 주요 기능

### 1. 3D 아바타 구현
- 3D 아바타 모델링 및 애니메이션 적용  
- 외부 SDK 연동을 통한 **립싱크(Lip Sync)** 기능 구현

### 2. VR 환경 구축
- Unity 기반 **몰입형 상담 공간 설계**  
- 2가지 상담 씬 구현
  - 해변  
    ![Beach](https://github.com/user-attachments/assets/3fbf2a2d-c500-46de-9a4e-e5e99ceb6543)
  - 숲  
    ![Forest](https://github.com/user-attachments/assets/0cb15164-e67d-4058-842a-5a32b006e854)

### 3. 상담 준비 시스템
- **PSS-10(Perceived Stress Scale)** 자가 진단 스트레스 설문 구현
- 설문 결과를 상담 프롬프트 구성에 활용  
- 극단적 응답 문항을 식별하여 AI 응답에 반영

### 4. AI 서비스 연동 (Microsoft Azure 기반)
- **STT (Speech-to-Text)**: 사용자의 음성을 실시간 텍스트로 변환  
- **ChatGPT API 연동**: 사용자를 위한 답변 생성  
- **TTS (Text-to-Speech)**: AI 응답을 음성으로 출력하여 아바타에 적용

---

## 💻 개발 환경 및 필요 사항
- Unity **2022.3.20f1**
- Microsoft **Azure Cognitive Services** API 키 필요  
  ![api 코드 예시](https://github.com/user-attachments/assets/1ea2c8c9-9167-479a-9cf2-739a61b29ede)
- **OpenXR 기반 VR 디바이스** 사용 권장 (예: Meta Quest 2), 마이크

---

## ⚠️ 주의 사항

본 프로젝트는 Microsoft Azure의 OpenAI 서비스를 활용하고 있으며, 기본 또는 무료 플랜에서는 다음과 같은 제약이 있을 수 있습니다:

- 빠르게 연속 요청 시, 응답 지연 또는 실패가 발생할 수 있습니다.
- 이는 Azure에서 설정한 요청 속도(Rate Limit) 및 처리량(Token Limit) 정책에 따른 현상입니다.
- 필요 시 요청 간 딜레이를 주거나, 상위 요금제로 전환하는 것을 고려해주세요.

> 예: `"No response"`, `"Timeout"`, `"RateLimitExceeded"` 등의 응답이 반복될 경우 위와 같은 제한으로 인해 발생할 수 있습니다.
