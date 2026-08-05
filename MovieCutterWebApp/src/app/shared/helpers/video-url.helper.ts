const youTubePathPatterns = [
  /^\/shorts\/([^/?#]+)/,
  /^\/embed\/([^/?#]+)/,
  /^\/live\/([^/?#]+)/,
  /^\/v\/([^/?#]+)/,
];

const instagramPathPattern = /^\/(?:[^/]+\/)?(?:p|reel|reels|tv)\/([^/?#]+)/;

const tikTokPathPatterns = [
  /^\/@[^/]+\/(?:video|photo)\/([^/?#]+)/,
  /^\/(?:t|v|embed)\/([^/?#]+)/,
];

// Skrócone linki TikToka (vm./vt.), gdzie kod znajduje się bezpośrednio w ścieżce.
const tikTokShortHosts = ['vm.tiktok.com', 'vt.tiktok.com'];

/**
 * Wyciąga identyfikator zasobu z linku do wideo (YouTube, Instagram, TikTok).
 * Zwraca `null`, gdy link jest niepoprawny lub serwis nie jest obsługiwany.
 */
export function extractVideoIdFromUrl(url: string): string | null {
  const parsed = parseUrl(url);
  if (!parsed) return null;

  const host = parsed.hostname.toLowerCase().replace(/^www\./, '');
  const path = parsed.pathname;

  if (host === 'youtu.be') {
    return firstSegment(path);
  }

  if (host === 'youtube.com' || host.endsWith('.youtube.com') || host === 'youtube-nocookie.com') {
    const idFromQuery = parsed.searchParams.get('v');
    if (idFromQuery) return idFromQuery;
    return matchFirst(path, youTubePathPatterns);
  }

  if (host === 'instagram.com' || host.endsWith('.instagram.com')) {
    return matchFirst(path, [instagramPathPattern]);
  }

  if (tikTokShortHosts.includes(host)) {
    return stripHtmlSuffix(firstSegment(path));
  }

  if (host === 'tiktok.com' || host.endsWith('.tiktok.com')) {
    return stripHtmlSuffix(matchFirst(path, tikTokPathPatterns));
  }

  return null;
}

function parseUrl(url: string): URL | null {
  const trimmed = url.trim();
  if (!trimmed) return null;

  const withProtocol = /^https?:\/\//i.test(trimmed) ? trimmed : `https://${trimmed}`;
  try {
    return new URL(withProtocol);
  } catch {
    return null;
  }
}

function matchFirst(path: string, patterns: RegExp[]): string | null {
  for (const pattern of patterns) {
    const match = path.match(pattern);
    if (match?.[1]) return match[1];
  }
  return null;
}

function firstSegment(path: string): string | null {
  const segment = path.split('/').filter(Boolean)[0];
  return segment ?? null;
}

function stripHtmlSuffix(id: string | null): string | null {
  return id ? id.replace(/\.html$/i, '') : null;
}
