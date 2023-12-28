/** @type {import('next').NextConfig} */
const nextConfig = {
    images: {
        remotePatterns: [
            {
                hostname: 'cdn.pixabay.com'
            }
        ]
    }
}

module.exports = nextConfig
