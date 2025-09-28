import React from "react";

export default function App() {
  return (
    
    <div className="min-h-screen grid place-items-center bg-[#F6F0E6]">
      {/* 휴대폰 프레임(장식) */}
      <div className="w-[390px] max-w-[92vw] rounded-[36px] bg-black p-3 shadow-2xl">
        <div className="rounded-[28px] overflow-hidden bg-[#FAEEDC]">
          {/* 상단 상태바 (장식) */}
          <div className="h-6 bg-[#0F172A]" />
          {/* 본문 */}
          <div className="min-h-[720px] px-6 pt-7 pb-8 flex flex-col">
            <TopBadges hearts={3} coins={120} />

            {/* 타이틀 mt값을 조절함으로서 간격 늘리기 줄이기 가능*/}
            <h1 className="mt-30 text-[32px] leading-[1] font-extrabold text-[#1F2937]">
              미니게임리워드앱
            </h1>

            {/* 주요 CTA */}
            <button
              className="mt-25 py-5 w-full rounded-full bg-[#4F86F7] text-white text-[20px] font-bold shadow-sm active:scale-[.99] transition"
              onClick={() => alert("즉시 시작: 다음 단계에 Stage1로 연결")}
            >
              즉시 시작
            </button>



            {/* 비활성(장식) 버튼들 */}
            <div className="mt-3 space-y-3">
              <Pill label="연습 모드" disabled />
              <Pill label="친구와 PvP" disabled />
              <Pill label="지갑" disabled />
            </div>

            {/* 하단 안내 문구 */}
            <p className="mt-auto text-center text-[12px] text-[#9CA3AF]">
              실패 또는 보상 수령 시 광고 1회
            </p>
          </div>

          {/* 하단 제스처바(장식) */}
          <div className="pb-4 pt-2 grid place-items-center bg-[#FAEEDC]">
            <div className="h-1.5 w-28 rounded-full bg-black/20" />
          </div>
        </div>
      </div>
    </div>
  );
}

function TopBadges({ hearts, coins }: { hearts: number; coins: number }) {
  return (
    <div className="flex items-center justify-between">
      <Badge>
        <span className="text-[18px]">❤️</span>
        <span className="ml-2 text-[16px] font-semibold text-[#334155]">{hearts}</span>
      </Badge>
      <Badge>
        <span className="text-[18px]">🟡</span>
        <span className="ml-2 text-[16px] font-semibold text-[#334155]">{coins}</span>
      </Badge>
    </div>
  );
}

function Badge({ children }: { children: React.ReactNode }) {
  return (
    <div className="px-4 py-2 rounded-[16px] bg-white/85 backdrop-blur border border-black/5 shadow-sm inline-flex items-center">
      {children}
    </div>
  );
}

function Pill({
  label,
  disabled,
  muted,
}: {
  label: string;
  disabled?: boolean;
  muted?: boolean;
}) {
  const base =
    "w-full py-4 rounded-[22px] text-[17px] font-semibold border shadow-sm";
  if (muted) {
    return (
      <div className={`${base} bg-white/85 text-[#374151] border-black/5`}>
        {label}
      </div>
    );
  }
  if (disabled) {
    return (
      <button
        aria-disabled
        className={`${base} bg-white text-[#94A3B8] border-black/5 cursor-not-allowed`}
        onClick={(e) => e.preventDefault()}
      >
        {label}
      </button>
    );
  }
  return (
    <button className={`${base} bg-white text-[#374151] border-black/5`}>
      {label}
    </button>
  );
}
